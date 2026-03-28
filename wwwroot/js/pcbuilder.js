let currentBuild = {
    CPU: null,
    RAM: null,
    VGA: null,
    Mainboard: null,
    PSU: null,
    SSD: null
};

async function loadComponents(type) {
    try {
        const res = await fetch(`/api/pcbuilder/components?type=${type}`);
        const data = await res.json();

        let html = "";
        data.forEach(p => {
            html += `
                <div class="part p-3 border rounded mb-2 cursor-pointer hover-bg" onclick="selectComponent('${type}', ${p.id}, '${p.name.replace(/'/g, "\\'")}', ${p.price}, '${p.imageUrl}')">
                    <img src="${p.imageUrl}" class="img-thumbnail me-2" style="width:50px;">
                    <div>
                        <strong>${p.name}</strong><br>
                        <span class="text-success">${p.price.toLocaleString()} ₫</span>
                    </div>
                </div>
            `;
        });

        document.getElementById("partsGrid").innerHTML = html;
    } catch (e) {
        document.getElementById("partsGrid").innerHTML = '<div class="alert alert-danger">Lỗi load linh kiện</div>';
    }
}

function selectComponent(type, id, name, price, imageUrl) {
    currentBuild[type] = { id, name, price, imageUrl };
    renderBuild();
    updateSlot(type);
}

function updateSlot(type) {
    const slotMap = {
        'CPU': 'cpuSlot',
        'RAM': 'ramSlot',
        'VGA': 'vgaSlot',
        'Mainboard': 'mainboardSlot',
        'PSU': 'psuSlot',
        'SSD': 'ssdSlot'
    };

    const slot = document.getElementById(slotMap[type]);
    const part = currentBuild[type];
    slot.innerHTML = `
        <div class="selected p-2">
            <strong>${part.name}</strong>
            <br><small class="text-success">${part.price.toLocaleString()} ₫</small>
        </div>
    `;
}

function renderBuild() {
    let total = 0;
    let html = "";
    Object.keys(currentBuild).forEach(key => {
        const part = currentBuild[key];
        if (part) {
            html += `<p><strong>${key}:</strong> ${part.name} - ${part.price.toLocaleString()} ₫</p>`;
            total += part.price;
        }
    });

    document.getElementById("selected").innerHTML = html;
    document.getElementById("total").innerText = total.toLocaleString() + " ₫";
}

async function autoBuild() {
    const budget = prompt("Ngân sách (VNĐ):") || "20000000";
    try {
        const res = await fetch(`/api/pcbuilder/auto-build?budget=${budget}`);
        const data = await res.json();

        currentBuild.CPU = data.CPU;
        currentBuild.GPU = data.GPU;
        currentBuild.RAM = data.RAM;
        currentBuild.SSD = data.SSD;
        currentBuild.Mainboard = data.Mainboard;
        currentBuild.PSU = data.PSU;

        renderBuild();
        Object.keys(currentBuild).forEach(key => {
            if (currentBuild[key]) updateSlot(key);
        });
    } catch (e) {
        alert('Lỗi AI build');
    }
}

function clearBuild() {
    Object.keys(currentBuild).forEach(key => currentBuild[key] = null);
    renderBuild();
    document.querySelectorAll('.component-slot').forEach(slot => {
        slot.innerHTML = slot.dataset.originalContent || 'Chọn linh kiện';
    });
}

// Init
document.querySelectorAll('.component-slot').forEach(slot => {
    slot.dataset.originalContent = slot.innerHTML;
    slot.onclick = () => loadComponents(slot.dataset.type);
});

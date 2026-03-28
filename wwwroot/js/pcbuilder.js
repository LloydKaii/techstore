let currentBuild = {
    cpu: null,
    gpu: null,
    ram: null,
    mainboard: null,
    psu: null,
    storage: null
};

// Mapping between slot type and component key
const typeMap = {
    'cpu': 'cpu',
    'ram': 'ram',
    'vga': 'gpu',
    'gpu': 'gpu',
    'mainboard': 'mainboard',
    'psu': 'psu',
    'ssd': 'storage',
    'storage': 'storage'
};

const slotMap = {
    'cpu': 'cpuSlot',
    'gpu': 'vgaSlot',
    'ram': 'ramSlot',
    'mainboard': 'mainboardSlot',
    'psu': 'psuSlot',
    'storage': 'ssdSlot'
};

async function loadComponents(slotType) {
    try {
        // Normalize the type
        const type = typeMap[slotType] || slotType;
        const categoryName = type.toUpperCase();

        const res = await fetch(`/api/pcbuilder/components?type=${categoryName}`);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);

        const data = await res.json();

        if (!Array.isArray(data)) {
            throw new Error('Invalid response format');
        }

        let html = "";
        if (data.length === 0) {
            html = '<div class="alert alert-warning">Không tìm thấy linh kiện ' + categoryName + '</div>';
        } else {
            data.forEach(p => {
                const imageUrl = p.imageUrl || "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=400&q=80";
                const escapedName = (p.name || '').replace(/'/g, "\\'").replace(/"/g, '&quot;');
                html += `
                    <div class="part p-3 border rounded mb-2 cursor-pointer hover-bg" onclick="selectComponent('${type}', ${p.id}, '${escapedName}', ${p.price}, '${imageUrl}')">
                        <img src="${imageUrl}" class="img-thumbnail me-2" style="width:50px; height:50px; object-fit:cover;" onerror="this.src='https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=400&q=80'">
                        <div>
                            <strong>${p.name}</strong><br>
                            <span class="text-success">${p.price.toLocaleString()} ₫</span>
                        </div>
                    </div>
                `;
            });
        }

        document.getElementById("partsGrid").innerHTML = html;
    } catch (e) {
        console.error('Error loading components:', e);
        document.getElementById("partsGrid").innerHTML = '<div class="alert alert-danger">Lỗi load linh kiện: ' + e.message + '</div>';
    }
}

function selectComponent(type, id, name, price, imageUrl) {
    const normalizedType = typeMap[type] || type;
    currentBuild[normalizedType] = { id, name, price, imageUrl };
    renderBuild();
    updateSlot(normalizedType);
}

function updateSlot(type) {
    const normalizedType = typeMap[type] || type;
    const slotId = slotMap[normalizedType];

    if (!slotId) {
        console.error('Unknown component type:', type);
        return;
    }

    const slot = document.getElementById(slotId);
    if (!slot) return;

    const part = currentBuild[normalizedType];
    if (!part) {
        slot.innerHTML = `<h5>${slotId.replace('Slot', '').toUpperCase()}</h5>`;
        return;
    }

    slot.innerHTML = `
        <div class="selected p-2">
            <img src="${part.imageUrl}" style="width: 80px; height: 80px; object-fit: cover; border-radius: 4px; margin-bottom: 8px;">
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
            html += `<p><strong>${key.toUpperCase()}:</strong> ${part.name}<br><span class="text-success">${part.price.toLocaleString()} ₫</span></p>`;
            total += part.price;
        }
    });

    const totalElement = document.getElementById("totalPrice");
    if (totalElement) {
        totalElement.innerText = total.toLocaleString("vi-VN") + " ₫";
    }
}

async function aiSuggest() {
    const budgetInput = prompt("Ngân sách (VNĐ) - mặc định 20,000,000:", "20000000");
    if (!budgetInput) return;

    const budget = parseInt(budgetInput.replace(/[^0-9]/g, ''));
    if (isNaN(budget) || budget <= 0) {
        alert('Vui lòng nhập số tiền hợp lệ');
        return;
    }

    try {
        const purposeSelect = prompt("Mục đích sử dụng:\n1. gaming\n2. office\n3. content\n(mặc định: balanced)", "balanced");
        const purpose = purposeSelect || "balanced";

        const res = await fetch(`/api/pcbuilder/auto-build?budget=${budget}&purpose=${purpose}`);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);

        const response = await res.json();
        console.log('AI Build Response:', response);

        // Read from response.build object
        if (response.build) {
            const build = response.build;

            if (build.cpu) {
                currentBuild.cpu = {
                    id: build.cpu.id,
                    name: build.cpu.name,
                    price: build.cpu.price,
                    imageUrl: "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=400&q=80"
                };
                updateSlot('cpu');
            }
            if (build.gpu) {
                currentBuild.gpu = {
                    id: build.gpu.id,
                    name: build.gpu.name,
                    price: build.gpu.price,
                    imageUrl: "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=400&q=80"
                };
                updateSlot('gpu');
            }
            if (build.ram) {
                currentBuild.ram = {
                    id: build.ram.id,
                    name: build.ram.name,
                    price: build.ram.price,
                    imageUrl: "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=400&q=80"
                };
                updateSlot('ram');
            }
            if (build.motherboard) {
                currentBuild.mainboard = {
                    id: build.motherboard.id,
                    name: build.motherboard.name,
                    price: build.motherboard.price,
                    imageUrl: "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=400&q=80"
                };
                updateSlot('mainboard');
            }
            if (build.psu) {
                currentBuild.psu = {
                    id: build.psu.id,
                    name: build.psu.name,
                    price: build.psu.price,
                    imageUrl: "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=400&q=80"
                };
                updateSlot('psu');
            }
            if (build.ssd || build.storage) {
                const storage = build.ssd || build.storage;
                currentBuild.storage = {
                    id: storage.id,
                    name: storage.name,
                    price: storage.price,
                    imageUrl: "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=400&q=80"
                };
                updateSlot('storage');
            }
        }

        renderBuild();

        if (response.warnings && response.warnings.length > 0) {
            alert('⚠️ ' + response.warnings.join('\n'));
        } else {
            alert('✅ AI gợi ý build thành công!');
        }
    } catch (e) {
        console.error('AI Build Error:', e);
        alert('Lỗi AI build: ' + e.message);
    }
}

function saveBuild() {
    const buildName = prompt("Tên build của bạn:", "My PC Build");
    if (!buildName) return;

    const components = {};
    Object.keys(currentBuild).forEach(key => {
        if (currentBuild[key]) {
            components[key + 'Id'] = currentBuild[key].id;
        }
    });

    fetch('/Build/Save', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            name: buildName,
            components: components
        })
    })
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                alert('✅ Build đã được lưu!');
                window.location.href = '/Build/MyBuilds';
            } else {
                alert('❌ ' + (data.error || 'Lỗi lưu build'));
            }
        })
        .catch(e => {
            console.error('Save error:', e);
            alert('❌ Lỗi lưu build: ' + e.message);
        });
}

function clearBuild() {
    Object.keys(currentBuild).forEach(key => currentBuild[key] = null);
    renderBuild();
    document.querySelectorAll('.component-slot').forEach(slot => {
        const type = slot.dataset.type;
        const normalizedType = typeMap[type] || type;
        const slotId = slotMap[normalizedType];
        const slotElement = document.getElementById(slotId);
        if (slotElement) {
            slotElement.innerHTML = `<h5>${slotId.replace('Slot', '').toUpperCase()}</h5>`;
        }
    });
    alert('✅ Đã xóa tất cả linh kiện được chọn');
}

// Init
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.component-slot').forEach(slot => {
        slot.dataset.originalContent = slot.innerHTML;
        slot.onclick = () => loadComponents(slot.dataset.type);
    });
});

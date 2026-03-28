/* TechStore Effects - Particles + Aurora + Neon */
(function() {
  'use strict';

  // ══ CANVAS PARTICLES ══════════════════════════════════
  const canvas = document.getElementById('ts-canvas');
  if (!canvas) return;
  const ctx = canvas.getContext('2d');
  let W, H, particles = [], mouseX = 0, mouseY = 0, animId;

  function resize() {
    W = canvas.width  = window.innerWidth;
    H = canvas.height = window.innerHeight;
  }
  resize();
  window.addEventListener('resize', resize);
  window.addEventListener('mousemove', e => { mouseX = e.clientX; mouseY = e.clientY; });

  const COLORS = [
    { r: 0,   g: 255, b: 255 },  // cyan
    { r: 123, g: 94,  b: 167 },  // purple
    { r: 255, g: 107, b: 157 },  // pink
    { r: 0,   g: 180, b: 255 },  // blue
  ];

  class Particle {
    constructor() { this.reset(true); }
    reset(init) {
      this.x    = Math.random() * W;
      this.y    = init ? Math.random() * H : (Math.random() < 0.5 ? -10 : H + 10);
      this.vx   = (Math.random() - 0.5) * 0.6;
      this.vy   = (Math.random() - 0.5) * 0.6;
      this.r    = Math.random() * 2 + 0.5;
      this.baseA = Math.random() * 0.6 + 0.15;
      this.a    = this.baseA;
      this.col  = COLORS[Math.floor(Math.random() * COLORS.length)];
      this.pulse = Math.random() * Math.PI * 2;
      this.pulseSpeed = Math.random() * 0.025 + 0.008;
      this.twinkle = Math.random() * Math.PI * 2;
      this.twinkleSpeed = Math.random() * 0.05 + 0.02;
    }
    update() {
      this.x += this.vx;
      this.y += this.vy;
      this.pulse += this.pulseSpeed;
      this.twinkle += this.twinkleSpeed;

      // Mouse attraction/repulsion
      const dx = this.x - mouseX, dy = this.y - mouseY;
      const dist = Math.hypot(dx, dy);
      if (dist < 150) {
        const force = (150 - dist) / 150 * 0.3;
        this.vx += (dx / dist) * force * 0.1;
        this.vy += (dy / dist) * force * 0.1;
      }

      // Speed limit
      const speed = Math.hypot(this.vx, this.vy);
      if (speed > 2) { this.vx = (this.vx / speed) * 2; this.vy = (this.vy / speed) * 2; }

      // Twinkle alpha
      this.a = this.baseA * (0.5 + 0.5 * Math.sin(this.twinkle));

      if (this.x < -20 || this.x > W + 20 || this.y < -20 || this.y > H + 20) this.reset(false);
    }
    draw() {
      const { r, g, b } = this.col;
      // Glow
      const grad = ctx.createRadialGradient(this.x, this.y, 0, this.x, this.y, this.r * 4);
      grad.addColorStop(0, `rgba(${r},${g},${b},${this.a})`);
      grad.addColorStop(0.4, `rgba(${r},${g},${b},${this.a * 0.3})`);
      grad.addColorStop(1, `rgba(${r},${g},${b},0)`);
      ctx.beginPath();
      ctx.arc(this.x, this.y, this.r * 4, 0, Math.PI * 2);
      ctx.fillStyle = grad;
      ctx.fill();
      // Core
      ctx.beginPath();
      ctx.arc(this.x, this.y, this.r, 0, Math.PI * 2);
      ctx.fillStyle = `rgba(${r},${g},${b},${this.a * 1.5})`;
      ctx.fill();
    }
  }

  // Init particles
  for (let i = 0; i < 120; i++) particles.push(new Particle());

  // Connection lines
  function drawLines() {
    for (let i = 0; i < particles.length; i++) {
      for (let j = i + 1; j < particles.length; j++) {
        const dx = particles[i].x - particles[j].x;
        const dy = particles[i].y - particles[j].y;
        const d  = Math.hypot(dx, dy);
        if (d < 120) {
          const a = 0.12 * (1 - d / 120);
          ctx.beginPath();
          ctx.moveTo(particles[i].x, particles[i].y);
          ctx.lineTo(particles[j].x, particles[j].y);
          ctx.strokeStyle = `rgba(0,255,255,${a})`;
          ctx.lineWidth = 0.7;
          ctx.stroke();
        }
      }
      // Mouse connection
      const mdx = particles[i].x - mouseX, mdy = particles[i].y - mouseY;
      const md  = Math.hypot(mdx, mdy);
      if (md < 120) {
        const a = 0.25 * (1 - md / 120);
        ctx.beginPath();
        ctx.moveTo(particles[i].x, particles[i].y);
        ctx.lineTo(mouseX, mouseY);
        ctx.strokeStyle = `rgba(0,255,255,${a})`;
        ctx.lineWidth = 1;
        ctx.stroke();
      }
    }
  }

  // Shooting stars
  let shootStars = [];
  class ShootingStar {
    constructor() { this.reset(); }
    reset() {
      this.x  = Math.random() * W;
      this.y  = Math.random() * H * 0.4;
      this.len = Math.random() * 80 + 40;
      this.speed = Math.random() * 8 + 6;
      this.angle = Math.PI / 4 + (Math.random() - 0.5) * 0.3;
      this.a  = 1;
      this.active = true;
    }
    update() {
      this.x += Math.cos(this.angle) * this.speed;
      this.y += Math.sin(this.angle) * this.speed;
      this.a -= 0.025;
      if (this.a <= 0) this.active = false;
    }
    draw() {
      const tx = this.x - Math.cos(this.angle) * this.len;
      const ty = this.y - Math.sin(this.angle) * this.len;
      const grad = ctx.createLinearGradient(tx, ty, this.x, this.y);
      grad.addColorStop(0, `rgba(255,255,255,0)`);
      grad.addColorStop(1, `rgba(255,255,255,${this.a})`);
      ctx.beginPath();
      ctx.moveTo(tx, ty);
      ctx.lineTo(this.x, this.y);
      ctx.strokeStyle = grad;
      ctx.lineWidth = 1.5;
      ctx.stroke();
    }
  }

  // Spawn shooting star occasionally
  setInterval(() => {
    if (Math.random() < 0.4) shootStars.push(new ShootingStar());
  }, 2000);

  // Main loop
  function loop() {
    ctx.clearRect(0, 0, W, H);
    particles.forEach(p => { p.update(); p.draw(); });
    drawLines();
    shootStars = shootStars.filter(s => s.active);
    shootStars.forEach(s => { s.update(); s.draw(); });
    animId = requestAnimationFrame(loop);
  }
  loop();

  // ══ SCROLL REVEAL ══════════════════════════════════════
  const revealObs = new IntersectionObserver(entries => {
    entries.forEach(e => {
      if (e.isIntersecting) {
        e.target.style.opacity = '1';
        e.target.style.transform = 'translateY(0)';
        revealObs.unobserve(e.target);
      }
    });
  }, { threshold: 0.06 });

  function initReveal() {
    document.querySelectorAll('.card-dark, [data-reveal]').forEach(el => {
      if (el.style.opacity !== '') return;
      el.style.opacity = '0';
      el.style.transform = 'translateY(28px)';
      el.style.transition = 'opacity .6s ease, transform .6s ease';
      revealObs.observe(el);
    });
  }
  initReveal();
  // Re-init for dynamically added elements
  setInterval(initReveal, 1000);

  // ══ NAVBAR GLOW ON SCROLL ══════════════════════════════
  const nav = document.getElementById('navbar');
  if (nav) {
    window.addEventListener('scroll', () => {
      nav.classList.toggle('scrolled', window.scrollY > 30);
    });
  }

  // ══ CURSOR GLOW ════════════════════════════════════════
  const cursorGlow = document.createElement('div');
  cursorGlow.style.cssText = 'position:fixed;width:300px;height:300px;border-radius:50%;background:radial-gradient(circle,rgba(0,255,255,.04) 0%,transparent 70%);pointer-events:none;z-index:0;transform:translate(-50%,-50%);transition:left .15s,top .15s;';
  document.body.appendChild(cursorGlow);
  window.addEventListener('mousemove', e => {
    cursorGlow.style.left = e.clientX + 'px';
    cursorGlow.style.top  = e.clientY + 'px';
  });

})();

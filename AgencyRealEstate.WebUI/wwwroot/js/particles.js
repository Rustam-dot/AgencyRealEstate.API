// ========================================
// INTERACTIVE PARTICLES BACKGROUND
// ========================================

class ParticleBackground {
    constructor() {
        this.container = null;
        this.particles = [];
        this.mouse = { x: 0, y: 0 };
        this.init();
    }

    init() {
        // Создаем контейнер для частиц
        this.container = document.createElement('div');
        this.container.id = 'particles-background';
        document.body.insertBefore(this.container, document.body.firstChild);

        // Добавляем эффект свечения
        const glow = document.createElement('div');
        glow.className = 'glow-effect';
        this.container.appendChild(glow);

        // Создаем частицы
        this.createParticles(50);

        // Отслеживаем движение мыши
        document.addEventListener('mousemove', (e) => {
            this.mouse.x = e.clientX;
            this.mouse.y = e.clientY;
            this.interactWithParticles();
        });

        // Анимация частиц
        this.animate();
    }

    createParticles(count) {
        for (let i = 0; i < count; i++) {
            const particle = document.createElement('div');
            particle.className = 'particle';
            
            // Случайный размер
            const size = Math.random() * 80 + 20;
            particle.style.width = `${size}px`;
            particle.style.height = `${size}px`;
            
            // Случайная позиция
            particle.style.left = `${Math.random() * 100}vw`;
            particle.style.top = `${Math.random() * 100}vh`;
            
            // Случайная задержка анимации
            particle.style.animationDelay = `${Math.random() * 15}s`;
            particle.style.animationDuration = `${Math.random() * 20 + 10}s`;
            
            // Случайный цвет с градиентом
            const colors = [
                'rgba(102, 126, 234, 0.3)',
                'rgba(118, 75, 162, 0.3)',
                'rgba(79, 172, 254, 0.3)',
                'rgba(240, 147, 251, 0.2)',
                'rgba(0, 242, 254, 0.2)'
            ];
            particle.style.background = `radial-gradient(circle, ${colors[Math.floor(Math.random() * colors.length)]} 0%, transparent 70%)`;
            
            this.container.appendChild(particle);
            this.particles.push({
                element: particle,
                x: parseFloat(particle.style.left),
                y: parseFloat(particle.style.top),
                vx: (Math.random() - 0.5) * 0.5,
                vy: (Math.random() - 0.5) * 0.5
            });
        }
    }

    interactWithParticles() {
        this.particles.forEach((particle, index) => {
            const rect = particle.element.getBoundingClientRect();
            const px = rect.left + rect.width / 2;
            const py = rect.top + rect.height / 2;
            
            const dx = this.mouse.x - px;
            const dy = this.mouse.y - py;
            const distance = Math.sqrt(dx * dx + dy * dy);
            
            if (distance < 200) {
                const force = (200 - distance) / 200;
                particle.element.style.transform = `scale(${1 + force * 0.5})`;
                particle.element.style.opacity = 0.8 + force * 0.2;
            } else {
                particle.element.style.transform = 'scale(1)';
                particle.element.style.opacity = '';
            }
        });
    }

    animate() {
        this.particles.forEach((particle) => {
            // Обновляем позицию
            particle.x += particle.vx;
            particle.y += particle.vy;
            
            // Границы экрана
            if (particle.x < 0 || particle.x > 100) particle.vx *= -1;
            if (particle.y < 0 || particle.y > 100) particle.vy *= -1;
            
            particle.element.style.left = `${particle.x}%`;
            particle.element.style.top = `${particle.y}%`;
        });
        
        requestAnimationFrame(() => this.animate());
    }
}

// Инициализация после загрузки страницы
document.addEventListener('DOMContentLoaded', () => {
    new ParticleBackground();
});

// ========================================
// SCROLL ANIMATIONS
// ========================================

const observerOptions = {
    threshold: 0.1,
    rootMargin: '0px 0px -100px 0px'
};

const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.classList.add('animate-in');
        }
    });
}, observerOptions);

// Наблюдаем за элементами с классом animate-on-scroll
document.querySelectorAll('.animate-on-scroll').forEach(el => {
    observer.observe(el);
});

// ========================================
// BUTTON RIPPLE EFFECT
// ========================================

document.querySelectorAll('.card-btn-pill, .search-btn, .btn-register').forEach(button => {
    button.addEventListener('click', function(e) {
        const ripple = document.createElement('span');
        const rect = this.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = e.clientX - rect.left - size / 2;
        const y = e.clientY - rect.top - size / 2;
        
        ripple.style.cssText = `
            position: absolute;
            width: ${size}px;
            height: ${size}px;
            left: ${x}px;
            top: ${y}px;
            background: rgba(255, 255, 255, 0.4);
            border-radius: 50%;
            transform: scale(0);
            animation: ripple 0.6s ease-out;
            pointer-events: none;
        `;
        
        this.style.position = 'relative';
        this.style.overflow = 'hidden';
        this.appendChild(ripple);
        
        setTimeout(() => ripple.remove(), 600);
    });
});

// Add ripple animation
const style = document.createElement('style');
style.textContent = `
    @keyframes ripple {
        to {
            transform: scale(4);
            opacity: 0;
        }
    }
    
    .animate-on-scroll {
        opacity: 0;
        transform: translateY(50px);
        transition: all 0.8s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    }
    
    .animate-on-scroll.animate-in {
        opacity: 1;
        transform: translateY(0);
    }
    
    .animate-on-scroll.delay-100 { transition-delay: 0.1s; }
    .animate-on-scroll.delay-200 { transition-delay: 0.2s; }
    .animate-on-scroll.delay-300 { transition-delay: 0.3s; }
    .animate-on-scroll.delay-400 { transition-delay: 0.4s; }
    .animate-on-scroll.delay-500 { transition-delay: 0.5s; }
`;
document.head.appendChild(style);

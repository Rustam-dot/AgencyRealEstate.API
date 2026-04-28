window.menu = {
    isOpen: false,
    timeline: null,

    // Инициализация: создаем "сценарий" анимации
    init: function () {
        const overlay = document.querySelector('.menu-overlay');
        const container = document.querySelector('.menu-container');
        const listItems = document.querySelectorAll('.menu-list li');

        if (!overlay || !container) return;

        // Удаляем старый таймлайн, если он был (нужно для горячей перезагрузки Blazor)
        if (this.timeline) this.timeline.kill();

        // Создаем новый таймлайн (изначально стоит на паузе)
        this.timeline = gsap.timeline({ paused: true });

        this.timeline
            .to(overlay, { duration: 0.3, opacity: 1, autoAlpha: 1, ease: 'power2.inOut' })
            .to(container, { duration: 0.4, x: '0%', ease: 'power2.out' }, '-=0.1');

        if (listItems.length > 0) {
            this.timeline.fromTo(listItems,
                { y: 20, opacity: 0 },
                { y: 0, opacity: 1, stagger: 0.1, duration: 0.3, ease: 'power2.out' },
                '-=0.2'
            );
        }
    },

    toggle: function () {
        if (!this.timeline) this.init(); // На всякий случай проверяем инициализацию

        if (this.isOpen) {
            this.timeline.reverse();
        } else {
            this.timeline.play();
        }
        this.isOpen = !this.isOpen;
    }
};
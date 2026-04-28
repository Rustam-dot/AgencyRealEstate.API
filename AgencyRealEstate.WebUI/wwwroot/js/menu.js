window.menu = {
    isOpen: false,
    timeline: null,
    overlay: null,

    init: function () {
        this.overlay = document.querySelector('.menu-overlay');

      
        this.timeline = gsap.timeline({ paused: true });
        this.timeline
            .to('.menu-overlay', { duration: 0.4, opacity: 1, visibility: 'visible', ease: 'power2.inOut' })
            .to('.menu-container', { duration: 0.4, x: '0%', ease: 'power2.out' }, '-=0.2')
            .fromTo('.menu li', { y: 30, opacity: 0 }, { y: 0, opacity: 1, stagger: 0.1, duration: 0.3, ease: 'power2.out' }, '-=0.1')
            .reverse();

     
        if (this.overlay) {
            this.overlay.addEventListener('click', this.close.bind(this));
        }
    },

    toggle: function () {
        if (!this.timeline) this.init();
        if (this.isOpen) {
            this.close();
        } else {
            this.open();
        }
    },

    open: function () {
        if (this.isOpen) return;
        if (this.overlay) this.overlay.classList.add('active');   
        this.timeline.play();
        this.isOpen = true;
    },

    close: function () {
        if (!this.isOpen) return;
        this.timeline.reverse();
        if (this.overlay) this.overlay.classList.remove('active'); 
        this.isOpen = false;
    }
};

window.addEventListener('load', function () {
    if (window.menu) window.menu.init();
});
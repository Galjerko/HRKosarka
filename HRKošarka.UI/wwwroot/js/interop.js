window.hrk = {
    scrollFeatured: function (id, direction) {
        var el = document.getElementById(id);
        if (!el) return;
        var card = el.firstElementChild;
        if (!card) return;
        var gap = parseInt(getComputedStyle(el).columnGap) || 10;
        el.scrollBy({ left: direction * (card.offsetWidth + gap), behavior: 'smooth' });
    }
};

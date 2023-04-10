window.GetWiewPortWidth = () => {
    return Math.max(document.documentElement.clientWidth, window.innerWidth || 0);
};

function openModal(nombre) {
    $(nombre).modal('show');
}


document.addEventListener("DOMContentLoaded", () => {

    const openBtn = document.getElementById("btnTornarVendedor");
    const modal = document.getElementById("tipoVendedorModal");
    const cancelBtn = document.getElementById("cancelTipoVendedor");

    if (openBtn && modal) {
        openBtn.addEventListener("click", () => {
            modal.classList.remove("hidden");
        });
    }

    cancelBtn?.addEventListener("click", () => {
        modal.classList.add("hidden");
    });

    modal?.addEventListener("click", e => {
        if (e.target === modal) {
            modal.classList.add("hidden");
        }
    });
});

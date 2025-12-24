document.addEventListener("DOMContentLoaded", () => {

    document.querySelectorAll(".btn-remover-filtro").forEach(btn => {
        btn.addEventListener("click", () => {

            fetch("/FiltrosFavoritos/Remover", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    idFiltro: parseInt(btn.dataset.id) // força número
                })
            })
                .then(r => r.json())
                .then(data => {
                    location.reload();
                });

        });
    });

});

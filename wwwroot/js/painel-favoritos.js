document.addEventListener("DOMContentLoaded", () => {

    document.querySelectorAll(".btn-cancelar-favorito").forEach(btn => {
        btn.addEventListener("click", () => {

            const id = parseInt(btn.dataset.id);
            console.log("A remover favorito com id:", id);

            fetch("/Painel/RemoverFavorito", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ idFavorito: id })
            })
                .then(r => {
                    console.log("Status:", r.status);
                    return r.text(); // NÃO json
                })
                .then(txt => {
                    console.log("Resposta:", txt);
                    location.reload(); // força refresh
                })
                .catch(err => {
                    console.error("Erro no fetch:", err);
                });

        });
    });

});

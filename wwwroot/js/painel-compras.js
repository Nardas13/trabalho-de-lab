document.addEventListener("DOMContentLoaded", () => {

    let compraSelecionada = null;

    function showToast(msg) {
        const box = document.createElement("div");
        box.className = "toast-auth";
        box.innerText = msg;
        document.body.appendChild(box);

        setTimeout(() => box.classList.add("show"), 10);
        setTimeout(() => box.classList.remove("show"), 2000);
        setTimeout(() => box.remove(), 2600);
    }

    // abrir modal
    document.querySelectorAll(".btn-cancelar-compra").forEach(btn => {
        btn.addEventListener("click", () => {
            compraSelecionada = parseInt(btn.dataset.id);
            document.getElementById("cancelModal").classList.remove("hidden");
        });
    });

    // fechar modal
    document.getElementById("closeCancel")?.addEventListener("click", () => {
        document.getElementById("cancelModal").classList.add("hidden");
    });

    // clicar fora
    document.getElementById("cancelModal")?.addEventListener("click", (e) => {
        if (e.target.id === "cancelModal") {
            document.getElementById("cancelModal").classList.add("hidden");
        }
    });

    // confirmar
    document.getElementById("confirmCancel")?.addEventListener("click", () => {
        if (!compraSelecionada) return;

        // fecha logo o modal (UX melhor)
        document.getElementById("cancelModal").classList.add("hidden");

        fetch("/Painel/CancelarCompra", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ idCompra: compraSelecionada })
        })
            .then(r => r.json())
            .then(data => {
                showToast(data.msg);
                setTimeout(() => location.reload(), 1200);
            });
    });

});

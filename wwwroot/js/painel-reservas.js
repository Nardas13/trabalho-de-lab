let reservaSelecionada = null;

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
document.querySelectorAll(".btn-cancelar-reserva").forEach(btn => {
    btn.addEventListener("click", () => {
        reservaSelecionada = parseInt(btn.dataset.id);
        document.getElementById("cancelModal").classList.remove("hidden");
    });
});

// fechar modal
document.getElementById("closeCancel").addEventListener("click", () => {
    document.getElementById("cancelModal").classList.add("hidden");
});

// fechar ao clicar fora
document.getElementById("cancelModal").addEventListener("click", (e) => {
    if (e.target.id === "cancelModal") {
        document.getElementById("cancelModal").classList.add("hidden");
    }
});

// confirmar cancelamento
document.getElementById("confirmCancel").addEventListener("click", () => {
    if (!reservaSelecionada) return;

    fetch("/Painel/CancelarReserva", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ idReserva: reservaSelecionada })
    })
        .then(r => r.json())
        .then(data => {
            showToast(data.msg);

            setTimeout(() => {
                location.reload();
            }, 1200);
        });
});

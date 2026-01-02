let visitaSelecionada = null;
let acaoSelecionada = null;

document.querySelectorAll(".confirmar-visita").forEach(btn => {
    btn.addEventListener("click", () => {
        visitaSelecionada = btn.dataset.id;
        acaoSelecionada = "confirmar";
        document.getElementById("modalTitle").innerText = "Confirmar Visita";
        document.getElementById("modalText").innerText =
            "Tens a certeza que queres confirmar esta visita?";
        document.getElementById("reservaModal").classList.remove("hidden");
    });
});

document.querySelectorAll(".cancelar-visita").forEach(btn => {
    btn.addEventListener("click", () => {
        visitaSelecionada = btn.dataset.id;
        acaoSelecionada = "cancelar";
        document.getElementById("modalTitle").innerText = "Cancelar Visita";
        document.getElementById("modalText").innerText =
            "Tens a certeza que queres cancelar esta visita?";
        document.getElementById("reservaModal").classList.remove("hidden");
    });
});

document.getElementById("confirmAction").addEventListener("click", () => {
    let url = acaoSelecionada === "confirmar"
        ? "/Painel/ConfirmarVisita"
        : "/Painel/CancelarVisitaVendedor";

    fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id: visitaSelecionada })
    })
        .then(() => location.reload());
});

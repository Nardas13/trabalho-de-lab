let reservaSelecionada = null;
let acaoSelecionada = null; // "confirmar" | "cancelar"

/* =========================
   TOAST
   ========================= */
function showToast(msg) {
    const box = document.createElement("div");
    box.className = "toast-auth";
    box.innerText = msg;
    document.body.appendChild(box);

    setTimeout(() => box.classList.add("show"), 10);
    setTimeout(() => box.classList.remove("show"), 2000);
    setTimeout(() => box.remove(), 2600);
}

/* =========================
   ABRIR MODAL - CANCELAR
   ========================= */
document.querySelectorAll(".cancelar-reserva").forEach(btn => {
    btn.addEventListener("click", () => {
        reservaSelecionada = parseInt(btn.dataset.id);
        acaoSelecionada = "cancelar";

        document.getElementById("modalTitle").innerText = "Cancelar Reserva";
        document.getElementById("modalText").innerText =
            "Tens a certeza que queres cancelar esta reserva?";

        document.getElementById("reservaModal").classList.remove("hidden");
    });
});

/* =========================
   ABRIR MODAL - CONFIRMAR
   ========================= */
document.querySelectorAll(".confirmar-reserva").forEach(btn => {
    btn.addEventListener("click", () => {
        reservaSelecionada = parseInt(btn.dataset.id);
        acaoSelecionada = "confirmar";

        document.getElementById("modalTitle").innerText = "Confirmar Reserva";
        document.getElementById("modalText").innerText =
            "Ao confirmar, esta reserva ficará ativa e o anúncio será reservado.";

        document.getElementById("reservaModal").classList.remove("hidden");
    });
});

/* =========================
   FECHAR MODAL
   ========================= */
document.getElementById("closeModal").addEventListener("click", () => {
    document.getElementById("reservaModal").classList.add("hidden");
    reservaSelecionada = null;
    acaoSelecionada = null;
});

/* fechar ao clicar fora */
document.getElementById("reservaModal").addEventListener("click", (e) => {
    if (e.target.id === "reservaModal") {
        document.getElementById("reservaModal").classList.add("hidden");
        reservaSelecionada = null;
        acaoSelecionada = null;
    }
});

/* =========================
   CONFIRMAR AÇÃO
   ========================= */
document.getElementById("confirmAction").addEventListener("click", () => {
    if (!reservaSelecionada || !acaoSelecionada) return;

    let url = "";

    if (acaoSelecionada === "confirmar") {
        url = "/Painel/ConfirmarReserva";
    }
    else if (acaoSelecionada === "cancelar") {
        url = "/Painel/CancelarReservaVendedor";
    }


    fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id: reservaSelecionada })
    })
        .then(r => {
            if (!r.ok) throw new Error("Erro na ação.");
            return r.text();
        })
        .then(() => {
            showToast("Ação realizada com sucesso.");

            setTimeout(() => {
                location.reload();
            }, 1200);
        })
        .catch(() => {
            showToast("Ocorreu um erro. Tenta novamente.");
        });
});

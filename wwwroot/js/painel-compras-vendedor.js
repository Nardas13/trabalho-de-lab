let compraSelecionada = null;
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
   ABRIR MODAL - CONFIRMAR
   ========================= */
document.querySelectorAll(".confirmar-compra").forEach(btn => {
    btn.addEventListener("click", () => {
        compraSelecionada = parseInt(btn.dataset.id);
        acaoSelecionada = "confirmar";

        document.getElementById("modalTitle").innerText = "Confirmar Venda";
        document.getElementById("modalText").innerText =
            "Ao confirmar, a venda ficará concluída e o anúncio será marcado como vendido.";

        document.getElementById("reservaModal").classList.remove("hidden");
    });
});

/* =========================
   ABRIR MODAL - CANCELAR
   ========================= */
document.querySelectorAll(".cancelar-compra").forEach(btn => {
    btn.addEventListener("click", () => {
        compraSelecionada = parseInt(btn.dataset.id);
        acaoSelecionada = "cancelar";

        document.getElementById("modalTitle").innerText = "Cancelar Venda";
        document.getElementById("modalText").innerText =
            "Tens a certeza que queres cancelar este pedido de compra?";

        document.getElementById("reservaModal").classList.remove("hidden");
    });
});

/* =========================
   FECHAR MODAL
   ========================= */
document.getElementById("closeModal").addEventListener("click", () => {
    fecharModal();
});

document.getElementById("reservaModal").addEventListener("click", (e) => {
    if (e.target.id === "reservaModal") {
        fecharModal();
    }
});

function fecharModal() {
    document.getElementById("reservaModal").classList.add("hidden");
    compraSelecionada = null;
    acaoSelecionada = null;
}

/* =========================
   CONFIRMAR AÇÃO
   ========================= */
document.getElementById("confirmAction").addEventListener("click", () => {
    if (!compraSelecionada || !acaoSelecionada) return;

    let url = "";

    if (acaoSelecionada === "confirmar") {
        url = "/Painel/ConfirmarVenda";
    }
    else if (acaoSelecionada === "cancelar") {
        url = "/Painel/CancelarVenda";
    }

    fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id: compraSelecionada })
    })
        .then(r => {
            if (!r.ok) throw new Error();
            return r.text();
        })
        .then(() => {
            showToast("Ação realizada com sucesso.");
            setTimeout(() => location.reload(), 1200);
        })
        .catch(() => {
            showToast("Ocorreu um erro. Tenta novamente.");
        });
});

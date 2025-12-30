const modal = document.getElementById("criarAnuncioModal");

// abrir modal criar anúncio
document.getElementById("openCriarAnuncio")?.addEventListener("click", () => {
    document.getElementById("criarAnuncioModal").classList.remove("hidden");
});

// fechar modal (botão)
document.getElementById("cancelCriar")?.addEventListener("click", () => {
    document.getElementById("criarAnuncioModal").classList.add("hidden");
});

// fechar modal (click fora)
document.getElementById("criarAnuncioModal")?.addEventListener("click", (e) => {
    if (e.target.id === "criarAnuncioModal") {
        document.getElementById("criarAnuncioModal").classList.add("hidden");
    }
});


// validar imagens
document.getElementById("criarAnuncioForm")?.addEventListener("submit", e => {
    const files = document.querySelector('input[name="Imagens"]').files;

    if (files.length !== 4) {
        e.preventDefault();
        alert("Tens de selecionar exatamente 4 imagens.");
    }
});

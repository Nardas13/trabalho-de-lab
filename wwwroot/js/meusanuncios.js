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





const imageBox = document.getElementById("imageBox");
const imageInput = document.getElementById("imageInput");
const form = document.getElementById("criarAnuncioForm");

let imagens = []; // mantém a ordem correta

function renderImages() {
    imageBox.innerHTML = "";

    imagens.forEach((file, index) => {
        const slot = document.createElement("div");
        slot.className = "image-slot";

        const img = document.createElement("img");
        img.src = URL.createObjectURL(file);

        slot.appendChild(img);
        imageBox.appendChild(slot);
    });

    if (imagens.length < 4) {
        const plus = document.createElement("div");
        plus.className = "image-slot plus";
        plus.innerText = "+";

        plus.addEventListener("click", () => imageInput.click());
        imageBox.appendChild(plus);
    }
}

imageInput.addEventListener("change", e => {
    const file = e.target.files[0];
    if (!file) return;

    if (imagens.length >= 4) return;

    imagens.push(file);
    renderImages();

    imageInput.value = ""; // reset
});

form.addEventListener("submit", e => {
    if (imagens.length !== 4) {
        e.preventDefault();
        alert("Tens de selecionar exatamente 4 imagens.");
        return;
    }

    // remover inputs antigos (se houver)
    form.querySelectorAll("input[name='Imagens']").forEach(i => i.remove());
    form.querySelectorAll("input[name='Ordem']").forEach(i => i.remove());

    imagens.forEach((file, index) => {
        const imgInput = document.createElement("input");
        imgInput.type = "file";
        imgInput.name = "Imagens";
        imgInput.files = createFileList(file);

        const ordemInput = document.createElement("input");
        ordemInput.type = "hidden";
        ordemInput.name = "Ordem";
        ordemInput.value = index + 1;

        form.appendChild(imgInput);
        form.appendChild(ordemInput);
    });
});

// helper para criar FileList
function createFileList(file) {
    const dt = new DataTransfer();
    dt.items.add(file);
    return dt.files;
}

// inicial
renderImages();

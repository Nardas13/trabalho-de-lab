const modal = document.getElementById("criarAnuncioModal");
const form = document.getElementById("criarAnuncioForm");
const imageBox = document.getElementById("imageBox");
const imageInput = document.getElementById("imageInput");
const submitBtn = document.getElementById("submitAnuncio");

function setErro(input, mostrar) {
    const error = input.parentElement.querySelector(".form-error");

    if (mostrar) {
        input.classList.add("invalid");
        if (error) error.classList.add("show");
    } else {
        input.classList.remove("invalid");
        if (error) error.classList.remove("show");
    }
}

function resetCriarModal() {

    form.reset();
    form.action = "/Painel/CriarAnuncio";

    document.querySelector("#criarAnuncioModal h2").textContent = "Novo Anúncio";
    submitBtn.textContent = "Publicar Anúncio";

    imagensExistentes = [];
    imagensNovas = [];
    renderImages();

    form.querySelector("input[name='IdAnuncio']")?.remove();

    document.querySelectorAll(".custom-select").forEach(sel => {
        sel.querySelector(".selected").textContent = "Selecionar";
    });

    tentouSubmeter = false;
}


document.addEventListener("DOMContentLoaded", () => {

    const openBtn = document.getElementById("openCriarAnuncio");
    const modal = document.getElementById("criarAnuncioModal");

    if (!openBtn || !modal) return;

    openBtn.addEventListener("click", () => {
        resetCriarModal();    

        const tipo = openBtn.dataset.tipo;          // "empresa" | "particular"
        const nif = openBtn.dataset.nif;
        const faturacao = openBtn.dataset.faturacao;

        // regra de negócio 
        if (tipo === "empresa") {

            if (!nif || nif.trim() === "") {
                showToast("Para criar anúncios como empresa tens de preencher o NIF nas definições da conta.");
                return;
            }

            if (!faturacao || faturacao.trim() === "") {
                showToast("Para criar anúncios como empresa tens de preencher os dados de faturação.");
                return;
            }
        }

        // tudo OK -> abre modal
        modal.classList.remove("hidden");
    });

    document.getElementById("cancelCriar")?.addEventListener("click", () => {
        modal.classList.add("hidden");
    });

    modal.addEventListener("click", e => {
        if (e.target === modal) modal.classList.add("hidden");
    });
});



document.querySelectorAll("#criarAnuncioModal .custom-select").forEach(select => {
    const selected = select.querySelector(".selected");
    const options = select.querySelectorAll(".custom-options div");
    const hiddenInput = select.parentElement.querySelector("input[type='hidden']");

    selected.addEventListener("click", e => {
        e.stopPropagation();

        document.querySelectorAll("#criarAnuncioModal .custom-select")
            .forEach(s => s !== select && s.classList.remove("active"));

        select.classList.toggle("active");
    });

    options.forEach(opt => {
        opt.addEventListener("click", () => {
            selected.textContent = opt.textContent;
            hiddenInput.value = opt.dataset.value;
            select.classList.remove("active");

            validarFormulario();
        });
    });
});

window.addEventListener("click", () => {
    document.querySelectorAll("#criarAnuncioModal .custom-select")
        .forEach(s => s.classList.remove("active"));
});



let imagens = [];
let imagensExistentes = [];
let imagensNovas = [];

function renderImages() {
    imageBox.innerHTML = "";

    // existentes
    imagensExistentes.forEach((url, index) => {
        const slot = document.createElement("div");
        slot.className = "image-slot";

        const img = document.createElement("img");
        img.src = url;

        slot.onclick = () => {
            imagensExistentes.splice(index, 1);
            renderImages();
        };

        slot.appendChild(img);
        imageBox.appendChild(slot);
    });

    // novas
    imagensNovas.forEach((file, index) => {
        const slot = document.createElement("div");
        slot.className = "image-slot";

        const img = document.createElement("img");
        img.src = URL.createObjectURL(file);

        slot.onclick = () => {
            imagensNovas.splice(index, 1);
            renderImages();
        };

        slot.appendChild(img);
        imageBox.appendChild(slot);
    });

    if (imagensExistentes.length + imagensNovas.length < 4) {
        const plus = document.createElement("div");
        plus.className = "image-slot plus";
        plus.textContent = "+";
        plus.onclick = () => imageInput.click();
        imageBox.appendChild(plus);
    }

    validarFormulario();
}


imageInput.addEventListener("change", e => {
    const file = e.target.files[0];
    if (!file) return;

    if (imagensExistentes.length + imagensNovas.length >= 4) return;

    imagensNovas.push(file);
    renderImages();
    imageInput.value = "";
});


function validarPreco() {
    const input = form.querySelector("input[name='Preco']");
    const valor = parseFloat(input.value);

    const invalido = isNaN(valor) || valor < 100 || valor > 10000000;
    setErro(input, invalido);

    return !invalido;
}

function validarFormulario() {
    let valido = true;

    // Só mostrar erros se tentou submeter
    if (!tentouSubmeter) return true;

    // inputs required
    form.querySelectorAll("input[required]").forEach(input => {
        const invalido = !input.value.trim();
        setErro(input, invalido);
        if (invalido) valido = false;
    });

    // dropdowns (hidden)
    form.querySelectorAll("input[type='hidden'][required]").forEach(input => {
        const select = input.parentElement.querySelector(".custom-select");
        const error = input.parentElement.querySelector(".form-error");

        const invalido = !input.value;
        select?.classList.toggle("invalid", invalido);
        error?.classList.toggle("show", invalido);

        if (invalido) valido = false;
    });

    // Ano
    if (!validarAno()) valido = false;

    // Preço
    if (!validarPreco()) valido = false;

    // Imagens
    const imageError = document.getElementById("imageError");
    const total = imagensExistentes.length + imagensNovas.length;

    if (total !== 4) {
        imageError.classList.add("show");
        valido = false;
    } else {
        imageError.classList.remove("show");
    }


    return valido;
}


let tentouSubmeter = false;

form.addEventListener("submit", e => {
    tentouSubmeter = true;

    const valido = validarFormulario();

    if (!valido) {
        e.preventDefault(); // bloqueia submit
        return;
    }

    // se chegou aqui, está tudo válido -> preparar imagens
    form.querySelectorAll("input[name='Imagens']").forEach(i => i.remove());

    imagensNovas.forEach(file => {
        const input = document.createElement("input");
        input.type = "file";
        input.name = "Imagens";

        const dt = new DataTransfer();
        dt.items.add(file);
        input.files = dt.files;

        form.appendChild(input);
    });

});


function validarAno() {
    const input = form.querySelector("input[name='Ano']");
    const ano = parseInt(input.value);
    const anoAtual = new Date().getFullYear();

    const invalido = !ano || ano < 1900 || ano > anoAtual;
    setErro(input, invalido);

    return !invalido;
}


// inicializar box das imagens (mostra o +)
renderImages();

["Ano", "Quilometragem"].forEach(name => {
    const input = form.querySelector(`input[name='${name}']`);
    if (!input) return;

    input.addEventListener("input", () => {
        input.value = input.value.replace(/\D/g, "");
    });
});


form.querySelectorAll("input, textarea").forEach(el => {
    el.addEventListener("input", () => {
        if (tentouSubmeter) validarFormulario();
    });
});

// ============================
// MODAL REMOVER ANÚNCIO
// ============================

const removerModal = document.getElementById("removerModal");
const removerIdInput = document.getElementById("removerId");
const removerTitulo = document.getElementById("removerTitulo");
const cancelRemover = document.getElementById("cancelRemover");

document.querySelectorAll(".open-remover-modal").forEach(btn => {
    btn.addEventListener("click", () => {

        if (btn.dataset.temReserva === "true") {
            showToast("Este anúncio tem uma reserva ativa. Aguarda o fim da reserva.");
            return;
        }

        if (btn.dataset.temVisitas === "true") {
            showToast("Este anúncio tem visitas agendadas. Cancela-as primeiro.");
            return;
        }

        if (btn.dataset.temCompras === "true") {
            showToast("Este anúncio tem compras pendentes associadas.");
            return;
        }

        // Pode abrir modal
        removerIdInput.value = btn.dataset.id;
        removerTitulo.textContent = btn.dataset.titulo;
        removerModal.classList.remove("hidden");
    });
});

cancelRemover?.addEventListener("click", () => {
    removerModal.classList.add("hidden");
});

removerModal?.addEventListener("click", e => {
    if (e.target === removerModal) {
        removerModal.classList.add("hidden");
    }
});




document.querySelectorAll('.open-editar-modal').forEach(btn => {
    btn.addEventListener('click', () => {

        const temReserva = btn.dataset.temReserva === "True";
        const temVisitas = btn.dataset.temVisitas === "True";
        const temCompras = btn.dataset.temCompras === "True";

        if (temReserva || temVisitas || temCompras) {
            showToast(
                "Este anúncio não pode ser editado porque tem visitas, reservas ou compras associadas."
            );
            return;
        }

        abrirEditarModal(btn.dataset.id);
    });
});
function abrirEditarModal(id) {

    fetch(`/Painel/GetAnuncio?id=${id}`)
        .then(r => {
            if (!r.ok) throw new Error();
            return r.json();
        })
        .then(a => {

            form.action = "/Painel/EditarAnuncio";
            document.querySelector("#criarAnuncioModal h2").textContent = "Editar Anúncio";
            submitBtn.textContent = "Guardar alterações";

            form.querySelector("input[name='Titulo']").value = a.titulo;
            form.querySelector("input[name='Preco']").value = a.preco;
            form.querySelector("input[name='Modelo']").value = a.modelo;
            form.querySelector("input[name='Ano']").value = a.ano;
            form.querySelector("input[name='Quilometragem']").value = a.quilometragem;
            form.querySelector("input[name='Localizacao']").value = a.localizacao ?? "";
            form.querySelector("textarea[name='Descricao']").value = a.descricao ?? "";

            ["Marca", "Categoria", "Combustivel", "Caixa"].forEach(campo => {
                const input = form.querySelector(`input[name='${campo}']`);
                input.value = a[campo.toLowerCase()];
                input.parentElement.querySelector(".selected").textContent = input.value;
            });

            let idInput = form.querySelector("input[name='IdAnuncio']");
            if (!idInput) {
                idInput = document.createElement("input");
                idInput.type = "hidden";
                idInput.name = "IdAnuncio";
                form.appendChild(idInput);
            }
            idInput.value = a.idAnuncio;

            
            imagensExistentes = [...a.imagens];
            imagensNovas = [];
            renderImages();

            tentouSubmeter = false;
            modal.classList.remove("hidden");
        })
        .catch(() => {
            showToast("Não foi possível editar este anúncio.");
        });
}

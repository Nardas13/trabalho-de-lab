/* ============================
   MODAL ALTERAR PASSWORD
============================ */

const pwdModal = document.getElementById("pwdModal");
const openPwdBtn = document.getElementById("openPwdModal");
const closePwdBtn = document.getElementById("closePwdModal");

if (openPwdBtn) {
    openPwdBtn.addEventListener("click", () => {
        pwdModal.classList.remove("hidden");
    });
}

if (closePwdBtn) {
    closePwdBtn.addEventListener("click", () => {
        pwdModal.classList.add("hidden");
    });
}

/* ============================
   TOGGLE PASSWORD
============================ */

function togglePwd(id, icon) {
    const input = document.getElementById(id);
    const show = "/imgs/eye.jpg";
    const hide = "/imgs/eye-off.jpg";

    const isHidden = input.type === "password";
    input.type = isHidden ? "text" : "password";
    icon.src = isHidden ? show : hide;
}

/* ============================
   VALIDAR PASSWORD EM TEMPO REAL
============================ */

const pwdAtual = document.getElementById("pwd-atual");
const pwdNova = document.getElementById("pwd-nova");
const pwdConfirmar = document.getElementById("pwd-confirmar");
const confirmBtn = document.getElementById("confirmPwdBtn");

function validarPasswordModal() {
    let valido = true;

    [pwdAtual, pwdNova, pwdConfirmar].forEach(i => {
        i.classList.remove("input-error");
    });

    // obrigatórios
    if (!pwdAtual.value || !pwdNova.value || !pwdConfirmar.value) {
        valido = false;
    }

    // nova != atual
    if (pwdNova.value && pwdAtual.value && pwdNova.value === pwdAtual.value) {
        pwdNova.classList.add("input-error");
        valido = false;
    }

    // nova == confirmar
    if (pwdNova.value !== pwdConfirmar.value) {
        pwdConfirmar.classList.add("input-error");
        valido = false;
    }

    confirmBtn.disabled = !valido;
}


// listeners
[pwdAtual, pwdNova, pwdConfirmar].forEach(input => {
    input?.addEventListener("input", validarPasswordModal);
});

/* ============================
   BLOQUEAR SUBMIT INVÁLIDO
============================ */

document.querySelector("#pwdModal form")?.addEventListener("submit", e => {
    if (confirmBtn.disabled) {
        e.preventDefault();
    }
});

/* ============================
   TELEFONE
============================ */

function restrictPhone(input) {
    input.value = input.value.replace(/\D/g, "");

    if (input.value.length > 9) {
        input.value = input.value.substring(0, 9);
    }
}


//erro
document.addEventListener("DOMContentLoaded", () => {
    const pwdAtual = document.getElementById("pwd-atual");
    if (pwdAtual && document.querySelector(".pwd-error")) {
        pwdAtual.classList.add("input-error");
    }
});

const formConta = document.getElementById("formConta");

formConta?.addEventListener("submit", e => {
    const nome = formConta.querySelector('input[name="Nome"]');
    const username = formConta.querySelector('input[name="Username"]');
    const telefone = formConta.querySelector('input[name="Telefone"]');
    const morada = formConta.querySelector('input[name="Morada"]');

    let valido = true;

    [nome, username, telefone, morada].forEach(i => {
        i.classList.remove("input-error");
    });

    if (!nome.value.trim()) {
        nome.classList.add("input-error");
        valido = false;
    }

    if (!username.value.trim()) {
        username.classList.add("input-error");
        valido = false;
    }

    if (!morada.value.trim()) {
        morada.classList.add("input-error");
        valido = false;
    }

    if (!telefone.value || telefone.value.length !== 9) {
        telefone.classList.add("input-error");
        valido = false;
    }

    if (!valido) {
        e.preventDefault();
        showToast("Verifica os campos destacados.");
    }
});

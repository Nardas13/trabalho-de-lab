document.querySelectorAll(".custom-select").forEach(select => {
    const selected = select.querySelector(".selected");
    const options = select.querySelector(".custom-options");

    selected.addEventListener("click", () => {
        select.classList.toggle("active");
    });

    options.querySelectorAll("div").forEach(option => {
        option.addEventListener("click", () => {
            selected.textContent = option.textContent;
            select.classList.remove("active");
        });
    });
});

window.addEventListener("click", e => {
    if (!e.target.closest(".custom-select")) {
        document.querySelectorAll(".custom-select").forEach(s => s.classList.remove("active"));
    }
});

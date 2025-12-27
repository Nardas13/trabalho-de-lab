function openBlockModal(userId) {
    document.getElementById("blockUserId").value = userId;
    document.getElementById("blockModal").classList.add("active");
}

function closeBlockModal() {
    document.getElementById("blockModal").classList.remove("active");
}

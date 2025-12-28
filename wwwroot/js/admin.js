function openBlockModal(userId) {
    document.getElementById("blockUserId").value = userId;
    document.getElementById("blockModal").classList.add("active");
}

function closeBlockModal() {
    document.getElementById("blockModal").classList.remove("active");
}

function openCreateAdminModal() {
    document.getElementById("createAdminModal").classList.add("active");
}

function closeCreateAdminModal() {
    document.getElementById("createAdminModal").classList.remove("active");
}

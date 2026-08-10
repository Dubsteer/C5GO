function requireLogin() {
    const overlay = document.getElementById("loginOverlay");
    if (!overlay) {
        return;
    }

    overlay.classList.remove("hidden");
    overlay.querySelector("a, button")?.focus();
}

function closeLoginModal() {
    document.getElementById("loginOverlay")?.classList.add("hidden");
}

document.addEventListener("DOMContentLoaded", () => {
    const overlay = document.getElementById("loginOverlay");
    document.querySelector("[data-close-login-modal]")?.addEventListener("click", closeLoginModal);

    overlay?.addEventListener("click", (event) => {
        if (event.target === overlay) {
            closeLoginModal();
        }
    });

    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape") {
            closeLoginModal();
        }
    });
});

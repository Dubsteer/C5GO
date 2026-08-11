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

    document.querySelectorAll("form[data-confirm]").forEach((form) => {
        form.addEventListener("submit", (event) => {
            if (!window.confirm(form.dataset.confirm)) {
                event.preventDefault();
            }
        });
    });

    document.querySelectorAll("img[data-fallback-image]").forEach((image) => {
        const applyFallback = () => {
            if (image.dataset.fallbackApplied === "true") {
                return;
            }

            image.dataset.fallbackApplied = "true";
            image.src = image.dataset.fallbackImage;
        };

        image.addEventListener("error", applyFallback);
        if (image.complete && image.naturalWidth === 0) {
            applyFallback();
        }
    });
});

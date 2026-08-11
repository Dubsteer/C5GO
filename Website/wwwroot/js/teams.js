(() => {
    const toastElement = document.getElementById("appToast");
    const toastBody = document.getElementById("toastMessage");

    const showToast = (message, type) => {
        if (!toastElement || !toastBody || !message) {
            return;
        }

        toastElement.classList.remove("toast-success", "toast-warning");
        toastElement.classList.add(type === "success" ? "toast-success" : "toast-warning");
        toastBody.textContent = message;
        new bootstrap.Toast(toastElement, { delay: 3000 }).show();
    };

    document.querySelectorAll("[data-require-steam-id]").forEach((button) => {
        button.addEventListener("click", () => {
            showToast(
                "Add your Steam ID in your profile before requesting to join a team.",
                "warning");
        });
    });

    document.querySelectorAll("[data-team-filter]").forEach((button) => {
        button.addEventListener("click", () => {
            const filter = button.dataset.teamFilter;
            document.querySelectorAll("[data-team-filter]").forEach((item) => {
                const isActive = item === button;
                item.classList.toggle("active", isActive);
                item.setAttribute("aria-pressed", String(isActive));
            });

            document.querySelectorAll(".team-item").forEach((card) => {
                card.hidden = filter !== "all" && card.dataset.status !== filter;
            });
        });
    });

    if (toastElement?.dataset.toastMessage) {
        showToast(toastElement.dataset.toastMessage, toastElement.dataset.toastType);
    }
})();

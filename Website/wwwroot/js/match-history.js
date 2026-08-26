(() => {
    const refreshIntervalMs = 5 * 60 * 1000;
    const loadedAt = Date.now();

    const refreshIfVisible = () => {
        if (document.visibilityState === "visible") {
            window.location.reload();
        }
    };

    window.setInterval(refreshIfVisible, refreshIntervalMs);
    document.addEventListener("visibilitychange", () => {
        if (Date.now() - loadedAt >= refreshIntervalMs) {
            refreshIfVisible();
        }
    });
})();

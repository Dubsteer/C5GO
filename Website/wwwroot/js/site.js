function parseUtcDate(value) {
    if (!value) {
        return null;
    }

    const hasTimeZone = /(?:z|[+-]\d{2}:\d{2})$/i.test(value);
    const parsed = new Date(hasTimeZone ? value : `${value}Z`);
    return Number.isNaN(parsed.getTime()) ? null : parsed;
}

function localizeTimes(root = document) {
    root.querySelectorAll("time[data-local-time]").forEach((element) => {
        const date = parseUtcDate(element.dateTime);
        if (!date) {
            return;
        }

        const format = element.dataset.localTime;
        const options = format === "date"
            ? { day: "2-digit", month: "short", year: "numeric" }
            : format === "time"
                ? { hour: "2-digit", minute: "2-digit" }
                : {
                    day: "2-digit",
                    month: "short",
                    year: "numeric",
                    hour: "2-digit",
                    minute: "2-digit"
                };

        element.textContent = new Intl.DateTimeFormat("en-GB", options).format(date);
        element.title = "Shown in your local time";
    });
}

window.c5g0LocalizeTimes = localizeTimes;

document.addEventListener("DOMContentLoaded", () => {
    localizeTimes();

    document.querySelectorAll("input[data-utc-datetime-input]").forEach((input) => {
        const utcDate = parseUtcDate(input.value);
        if (!utcDate) {
            return;
        }

        const pad = (value) => String(value).padStart(2, "0");
        input.value = `${utcDate.getFullYear()}-${pad(utcDate.getMonth() + 1)}-${pad(utcDate.getDate())}T${pad(utcDate.getHours())}:${pad(utcDate.getMinutes())}`;

        input.form?.addEventListener("submit", () => {
            const localDate = new Date(input.value);
            if (!Number.isNaN(localDate.getTime())) {
                input.value = localDate.toISOString().slice(0, 16);
            }
        });
    });

    const confirmDialog = document.getElementById("confirmDialog");
    const confirmMessage = confirmDialog?.querySelector("[data-confirm-dialog-message]");
    const confirmAccept = confirmDialog?.querySelector("[data-confirm-dialog-accept]");
    const confirmCancel = confirmDialog?.querySelector("[data-confirm-dialog-cancel]");
    let pendingForm = null;
    let pendingSubmitter = null;

    document.addEventListener("submit", (event) => {
        const form = event.target;
        if (!(form instanceof HTMLFormElement) || form.dataset.confirmed === "true") {
            return;
        }

        const message = form.dataset.confirm ?? form.dataset.confirmMessage;
        if (!message || !confirmDialog || !confirmMessage) {
            return;
        }

        event.preventDefault();
        event.stopImmediatePropagation();
        pendingForm = form;
        pendingSubmitter = event.submitter instanceof HTMLElement ? event.submitter : null;
        confirmMessage.textContent = message;
        confirmDialog.showModal();
        confirmCancel?.focus();
    }, true);

    confirmAccept?.addEventListener("click", () => {
        if (!pendingForm) {
            confirmDialog?.close();
            return;
        }

        const form = pendingForm;
        const submitter = pendingSubmitter;
        pendingForm = null;
        pendingSubmitter = null;
        form.dataset.confirmed = "true";
        confirmDialog?.close();
        form.requestSubmit(submitter);
        delete form.dataset.confirmed;
    });

    confirmCancel?.addEventListener("click", () => {
        pendingForm = null;
        pendingSubmitter = null;
        confirmDialog?.close();
    });

    confirmDialog?.addEventListener("click", (event) => {
        if (event.target === confirmDialog) {
            pendingForm = null;
            pendingSubmitter = null;
            confirmDialog.close();
        }
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

    const notification = document.querySelector("[data-notification-live]");
    if (notification) {
        const refreshNotificationCount = async () => {
            if (document.visibilityState !== "visible") {
                return;
            }

            try {
                const response = await fetch(notification.dataset.countUrl, {
                    credentials: "same-origin",
                    headers: { "X-Requested-With": "XMLHttpRequest" }
                });
                if (!response.ok) {
                    return;
                }

                const { unreadCount } = await response.json();
                const badge = notification.querySelector("[data-notification-badge]");
                const count = notification.querySelector("[data-notification-count]");
                const summary = notification.querySelector("[data-notification-summary]");
                const trigger = notification.querySelector("[data-notification-trigger]");
                badge?.classList.toggle("d-none", unreadCount === 0);
                if (count) {
                    count.textContent = unreadCount > 99 ? "99+" : String(unreadCount);
                }
                if (summary) {
                    summary.textContent = unreadCount === 0
                        ? "You're all caught up"
                        : `${unreadCount} unread`;
                }
                trigger?.setAttribute(
                    "aria-label",
                    unreadCount === 0 ? "Notifications" : `Notifications, ${unreadCount} unread`);
            } catch {
                // Keep the last known count when the network is temporarily unavailable.
            }
        };

        window.setInterval(refreshNotificationCount, 30 * 1000);
        document.addEventListener("visibilitychange", refreshNotificationCount);
    }
});

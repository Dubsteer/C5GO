document.addEventListener("DOMContentLoaded", () => {
    document.addEventListener("submit", (event) => {
        const form = event.target;
        if (form instanceof HTMLFormElement &&
            form.dataset.confirm &&
            !form.closest("[data-community-details]") &&
            !window.confirm(form.dataset.confirm)) {
            event.preventDefault();
        }
    });

    const details = document.querySelector("[data-community-details]");
    if (!details) {
        return;
    }

    const status = details.querySelector("[data-community-status]");
    const commentsSection = details.querySelector("[data-community-comments]");

    const setStatus = (message, isError = false) => {
        if (!status) {
            return;
        }

        status.textContent = message;
        status.classList.toggle("is-error", isError);
        status.hidden = message.length === 0;
    };

    const readError = async (response, fallback) => {
        if (response.headers.get("content-type")?.includes("application/json")) {
            const body = await response.json();
            return body.message ?? fallback;
        }

        return fallback;
    };

    details.addEventListener("click", (event) => {
        const spoiler = event.target.closest("[data-spoiler-media]");
        if (spoiler?.classList.contains("is-spoiler")) {
            spoiler.classList.remove("is-spoiler");
            spoiler.querySelector(".spoiler-reveal")?.remove();
            return;
        }

        const replyToggle = event.target.closest("[data-reply-toggle]");
        if (replyToggle) {
            const target = document.getElementById(replyToggle.dataset.replyToggle);
            if (target) {
                target.hidden = !target.hidden;
                if (!target.hidden) {
                    target.querySelector("textarea")?.focus({ preventScroll: true });
                }
            }
            return;
        }

        const reportToggle = event.target.closest("[data-report-toggle]");
        if (reportToggle) {
            const target = document.getElementById(reportToggle.dataset.reportToggle);
            if (target) {
                target.hidden = !target.hidden;
                if (!target.hidden) {
                    target.querySelector("select")?.focus({ preventScroll: true });
                }
            }
        }
    });

    details.addEventListener("submit", async (event) => {
        const form = event.target;
        if (!(form instanceof HTMLFormElement)) {
            return;
        }

        if (form.dataset.confirm && !window.confirm(form.dataset.confirm)) {
            event.preventDefault();
            return;
        }

        if (form.matches("[data-community-vote]")) {
            event.preventDefault();
            const submitter = event.submitter;
            if (!(submitter instanceof HTMLButtonElement)) {
                return;
            }

            submitter.disabled = true;
            try {
                const data = new FormData(form);
                data.set(submitter.name, submitter.value);
                const response = await fetch(form.action, {
                    method: "POST",
                    body: data,
                    credentials: "same-origin",
                    headers: { "X-Requested-With": "XMLHttpRequest" }
                });

                if (!response.ok) {
                    throw new Error(await readError(response, "The vote could not be saved."));
                }

                const body = await response.json();
                form.querySelector("[data-vote-score]").textContent = body.score;
                form.querySelectorAll(".vote-button").forEach((button) => {
                    button.classList.toggle(
                        "is-active",
                        button === submitter && !button.classList.contains("is-active"));
                });
                setStatus("");
            } catch (error) {
                setStatus(error instanceof Error ? error.message : "The vote failed.", true);
            } finally {
                submitter.disabled = false;
            }
            return;
        }

        if (form.matches("[data-community-report]")) {
            event.preventDefault();
            const submitButton = event.submitter;
            submitButton?.setAttribute("disabled", "disabled");
            try {
                const response = await fetch(form.action, {
                    method: "POST",
                    body: new FormData(form),
                    credentials: "same-origin",
                    headers: { "X-Requested-With": "XMLHttpRequest" }
                });

                if (!response.ok) {
                    throw new Error(await readError(response, "The report could not be submitted."));
                }

                const body = await response.json();
                form.reset();
                form.hidden = true;
                setStatus(body.message ?? "Report submitted.");
            } catch (error) {
                setStatus(error instanceof Error ? error.message : "The report failed.", true);
            } finally {
                submitButton?.removeAttribute("disabled");
            }
            return;
        }

        if (!form.matches("[data-community-comment-form]")) {
            return;
        }

        event.preventDefault();
        const submitButton = event.submitter;
        submitButton?.setAttribute("disabled", "disabled");
        try {
            const response = await fetch(form.action, {
                method: "POST",
                body: new FormData(form),
                credentials: "same-origin",
                headers: { "X-Requested-With": "XMLHttpRequest" }
            });

            if (!response.ok) {
                throw new Error(await readError(response, "The comment could not be saved."));
            }

            const html = await response.text();
            const template = document.createElement("template");
            template.innerHTML = html.trim();
            const replacement = template.content.querySelector("[data-community-comment-list]");
            const current = commentsSection?.querySelector("[data-community-comment-list]");
            if (!replacement || !current) {
                throw new Error("Comments could not be refreshed. Reload the page and try again.");
            }

            current.replaceWith(replacement);
            form.reset();
            if (form.classList.contains("community-reply-form")) {
                form.hidden = true;
            }
            setStatus(response.headers.get("X-Community-Message") ?? "Comments updated.");
        } catch (error) {
            setStatus(error instanceof Error ? error.message : "The request failed.", true);
        } finally {
            submitButton?.removeAttribute("disabled");
        }
    });
});

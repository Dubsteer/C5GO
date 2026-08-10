document.addEventListener("DOMContentLoaded", () => {
    const section = document.querySelector("[data-comment-section]");
    if (!section) {
        return;
    }

    const status = section.querySelector("[data-comment-status]");

    const setStatus = (message, isError = false) => {
        if (!status) {
            return;
        }

        status.textContent = message;
        status.classList.toggle("comment-status-error", isError);
        status.hidden = message.length === 0;
    };

    const setRepliesExpanded = (commentId, isExpanded) => {
        const button = section.querySelector(`[data-reply-toggle][data-comment-id="${commentId}"]`);
        if (!button) {
            return;
        }

        const replies = document.getElementById(button.getAttribute("aria-controls"));
        if (!replies) {
            return;
        }

        button.setAttribute("aria-expanded", String(isExpanded));
        button.textContent = `${isExpanded ? "Hide" : "Show"} replies (${button.dataset.replyCount})`;
        replies.hidden = !isExpanded;
    };

    section.addEventListener("click", (event) => {
        const button = event.target.closest("[data-reply-toggle]");
        if (!button || !section.contains(button)) {
            return;
        }

        const isExpanded = button.getAttribute("aria-expanded") === "true";
        setRepliesExpanded(button.dataset.commentId, !isExpanded);
    });

    section.addEventListener("submit", async (event) => {
        const form = event.target.closest("[data-async-comment-form]");
        if (!form || !section.contains(form)) {
            return;
        }

        event.preventDefault();

        if (form.dataset.confirmMessage && !window.confirm(form.dataset.confirmMessage)) {
            return;
        }

        const expandedComments = new Set(
            Array.from(section.querySelectorAll("[data-reply-toggle][aria-expanded=\"true\"]"))
                .map((button) => button.dataset.commentId)
        );

        if (form.dataset.expandComment) {
            expandedComments.add(form.dataset.expandComment);
        }

        const focusKey = form.dataset.focusAfter;
        const submitButton = event.submitter ?? form.querySelector("button[type=\"submit\"]");
        submitButton?.setAttribute("disabled", "disabled");
        form.setAttribute("aria-busy", "true");
        setStatus("");

        try {
            const response = await fetch(form.action, {
                method: "POST",
                body: new FormData(form),
                credentials: "same-origin",
                headers: {
                    "X-Requested-With": "XMLHttpRequest"
                }
            });

            if (!response.ok) {
                let message = "The comment could not be saved. Try again.";
                if (response.headers.get("content-type")?.includes("application/json")) {
                    const error = await response.json();
                    message = error.message ?? message;
                }

                throw new Error(message);
            }

            const html = await response.text();
            const template = document.createElement("template");
            template.innerHTML = html.trim();
            const replacement = template.content.querySelector("[data-comments-region]");
            const currentRegion = section.querySelector("[data-comments-region]");

            if (!replacement || !currentRegion) {
                throw new Error("The comments could not be refreshed. Reload the page and try again.");
            }

            currentRegion.replaceWith(replacement);

            if (form.dataset.focusAfter === "comment-input") {
                form.reset();
            }

            expandedComments.forEach((commentId) => setRepliesExpanded(commentId, true));

            const commentCount = section.querySelectorAll(".comment-card").length;
            const countElement = section.querySelector(".comment-count");
            if (countElement) {
                countElement.textContent = String(commentCount);
            }

            const focusTarget = Array.from(section.querySelectorAll("[data-comment-focus]"))
                .find((element) => element.dataset.commentFocus === focusKey);
            focusTarget?.focus({ preventScroll: true });

            setStatus(form.dataset.successMessage ?? "Comments updated.");
        } catch (error) {
            setStatus(error instanceof Error ? error.message : "The request failed. Try again.", true);
        } finally {
            submitButton?.removeAttribute("disabled");
            form.removeAttribute("aria-busy");
        }
    });
});

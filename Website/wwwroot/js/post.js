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
        const replyCount = Number(button.dataset.replyCount);
        const replyLabel = replyCount === 1 ? "reply" : "replies";
        button.textContent = `${isExpanded ? "Hide" : "View"} ${replyCount} ${replyLabel}`;
        replies.hidden = !isExpanded;
    };

    const setReplyComposerExpanded = (commentId, isExpanded, focusInput = false) => {
        const form = document.getElementById(`reply-form-${commentId}`);
        if (!form) {
            return;
        }

        section.querySelectorAll(`[data-reply-composer-toggle][data-comment-id="${commentId}"]`)
            .forEach((control) => {
                if (control.hasAttribute("aria-expanded")) {
                    control.setAttribute("aria-expanded", String(isExpanded));
                }
            });

        form.hidden = !isExpanded;
        if (isExpanded && focusInput) {
            form.querySelector("textarea")?.focus({ preventScroll: true });
        }
    };

    section.addEventListener("click", (event) => {
        const composerToggle = event.target.closest("[data-reply-composer-toggle]");
        if (composerToggle && section.contains(composerToggle)) {
            const commentId = composerToggle.dataset.commentId;
            const form = document.getElementById(`reply-form-${commentId}`);
            setReplyComposerExpanded(commentId, form?.hidden ?? true, true);
            return;
        }

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
        const expandedComposers = new Set(
            Array.from(section.querySelectorAll(".reply-form:not([hidden])"))
                .map((replyForm) => replyForm.dataset.commentId)
        );

        if (form.dataset.expandComment) {
            expandedComments.add(form.dataset.expandComment);
            expandedComposers.add(form.dataset.expandComment);
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
            expandedComposers.forEach((commentId) => setReplyComposerExpanded(commentId, true));

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

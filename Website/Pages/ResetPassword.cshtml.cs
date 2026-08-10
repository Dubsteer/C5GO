using LogicLayer.FormModels;
using LogicLayer.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Website.Services;

namespace Website.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager userManager;
        private readonly PasswordResetTokenService tokenService;

        public ResetPasswordModel(
            UserManager userManager,
            PasswordResetTokenService tokenService)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
        }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; } = string.Empty;

        [BindProperty]
        public ResetPasswordFormModel Form { get; set; } = new();

        public bool IsTokenValid { get; private set; }

        public IActionResult OnGet()
        {
            IsTokenValid = IsCurrentTokenValid();
            return Page();
        }

        public IActionResult OnPost()
        {
            var payload = tokenService.ReadToken(Token);
            IsTokenValid = IsCurrentTokenValid(payload);

            if (!IsTokenValid)
                return Page();

            if (!ModelState.IsValid)
                return Page();

            if (!userManager.ResetPassword(
                    payload!.UserId,
                    payload.CurrentPasswordHash,
                    Form.NewPassword))
            {
                IsTokenValid = false;
                return Page();
            }

            return RedirectToPage("/ResetPasswordConfirmation");
        }

        private bool IsCurrentTokenValid()
        {
            return IsCurrentTokenValid(tokenService.ReadToken(Token));
        }

        private bool IsCurrentTokenValid(PasswordResetTokenPayload? payload)
        {
            if (payload == null)
                return false;

            var user = userManager.GetUserById(payload.UserId);
            return user is { EmailConfirmed: true } &&
                   string.Equals(
                       user.Password,
                       payload.CurrentPasswordHash,
                       StringComparison.Ordinal);
        }
    }
}

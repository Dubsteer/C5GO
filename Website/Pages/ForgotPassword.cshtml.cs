using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Website.Services;

namespace Website.Pages
{
    [EnableRateLimiting("password-reset")]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager userManager;
        private readonly EmailService emailService;
        private readonly PasswordResetTokenService tokenService;
        private readonly TurnstileService turnstileService;
        private readonly ILogger<ForgotPasswordModel> logger;

        public ForgotPasswordModel(
            UserManager userManager,
            EmailService emailService,
            PasswordResetTokenService tokenService,
            TurnstileService turnstileService,
            ILogger<ForgotPasswordModel> logger)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.tokenService = tokenService;
            this.turnstileService = turnstileService;
            this.logger = logger;
        }

        [BindProperty]
        public ForgotPasswordFormModel Form { get; set; } = new();

        public string TurnstileSiteKey => turnstileService.SiteKey;

        public IActionResult OnGet()
        {
            return User.Identity?.IsAuthenticated == true
                ? RedirectToPage("/Index")
                : Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToPage("/Index");

            if (!ModelState.IsValid)
                return Page();

            var turnstileToken = Request.Form["cf-turnstile-response"].ToString();
            var isHuman = await turnstileService.ValidateAsync(
                turnstileToken,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.RequestAborted);
            if (!isHuman)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Verification could not be completed. Please try again.");
                return Page();
            }

            var user = userManager.GetUserByEmail(Form.Email);
            if (user is { Id: int userId, EmailConfirmed: true })
            {
                try
                {
                    var token = tokenService.CreateToken(userId, user.Password);
                    await emailService.SendPasswordResetEmail(user.Gmail, token);
                }
                catch (Exception exception)
                {
                    logger.LogWarning(exception, "The password reset email could not be sent.");
                }
            }

            return RedirectToPage("/ForgotPasswordConfirmation");
        }
    }
}

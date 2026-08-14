using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Exceptions;
using LogicLayer.Services;
using Website.Services;

namespace Website.Pages
{
    [EnableRateLimiting("register")]
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public FullUserFormModel FullUserFormModel { get; set; } = new();

        private readonly UserManager userManager;
        private readonly EmailService emailService;
        private readonly TurnstileService turnstileService;
        private readonly ILogger<RegisterModel> logger;

        public RegisterModel(
            UserManager userManager,
            EmailService emailService,
            TurnstileService turnstileService,
            ILogger<RegisterModel> logger)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.turnstileService = turnstileService;
            this.logger = logger;
        }

        public string TurnstileSiteKey => turnstileService.SiteKey;

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
                return Redirect("/");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
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

            var user = new User(
                FullUserFormModel.Firstname,
                FullUserFormModel.Lastname,
                FullUserFormModel.Age.GetValueOrDefault(),
                FullUserFormModel.Username,
                FullUserFormModel.Gmail,
                FullUserFormModel.Password,
                false
            );

            try
            {
                userManager.CreateUser(user);

                var token = user.EmailToken
                    ?? throw new InvalidOperationException("Verification token was not created.");

                await emailService.SendVerificationEmail(
                    user.Gmail,
                    token
                );
            }
            catch (UsernameAlreadyInUseException)
            {
                ModelState.AddModelError(
                    "FullUserFormModel.Username",
                    "This username is already taken."
                );
                return Page();
            }
            catch (EmailAlreadyInUseException)
            {
                ModelState.AddModelError(
                    "FullUserFormModel.Gmail",
                    "This email is already registered."
                );
                return Page();
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, "Registration could not be completed.");
                return StatusCode(500);
            }

            return Redirect("/RegisterSuccess");
        }
    }
}

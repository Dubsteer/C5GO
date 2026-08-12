using System.Security.Claims;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Website.Services;

namespace Website.Pages
{
    [EnableRateLimiting("login")]
    public class LoginModel : PageModel
    {
        private readonly UserManager userManager;
        private readonly UserRoleClaimsService roleClaimsService;

        public LoginModel(
            UserManager userManager,
            UserRoleClaimsService roleClaimsService)
        {
            this.userManager = userManager;
            this.roleClaimsService = roleClaimsService;
        }

        [BindProperty]
        public LoginFormModel LoginFormModel { get; set; } = new();

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToPage("Index");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            User? user = userManager.GetLoginUser(
                LoginFormModel.Username,
                LoginFormModel.Password);

            if (user?.Id == null)
            {
                ViewData["Error"] =
                    "Invalid credentials or email not verified. Please check your email.";

                LoginFormModel = new LoginFormModel();
                ModelState.Clear();
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new("id", user.Id.Value.ToString())
            };

            claims.AddRange(roleClaimsService.CreateRoleClaims(user));

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToPage("Index");
        }
    }
}

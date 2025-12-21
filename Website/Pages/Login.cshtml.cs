using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Website.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginFormModel LoginFormModel { get; set; }

        private readonly UserManager userManager;

        public LoginModel(UserManager userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToPage("Index");

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            // ? GetLoginUser sada NE baca exception
            User? user = userManager.GetLoginUser(
                LoginFormModel.Username,
                LoginFormModel.Password
            );

            if (user == null)
            {
                // ? JEDNA poruka za oba slu?aja:
                // - pogrešni kredencijali
                // - email nije verifikovan
                ViewData["Error"] =
                    "Invalid credentials or email not verified. Please check your email.";

                LoginFormModel = new LoginFormModel();
                ModelState.Clear();
                return Page();
            }

            // ? LOGIN USPJEŠAN
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("username", user.Username),
                new Claim("id", user.Id.Value.ToString()),
                new Claim("isAdmin", user.IsAdmin.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );

            return RedirectToPage("Index");
        }
    }
}

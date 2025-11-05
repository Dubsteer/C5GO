using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Website.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginFormModel LoginFormModel { get; set; }

        public readonly UserManager userManager;

        public LoginModel(UserManager userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult OnGet()
        {
            // allow only anonymus users
            if (User.Identity.IsAuthenticated)
                return new RedirectToPageResult("Index");

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            User? user;

            try
            {
                user = userManager.GetLoginUser(LoginFormModel.Username, LoginFormModel.Password);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // internal server error
                return StatusCode(500);
            }

            if (user is null)
            {
                ViewData["Error"] = "No user found with provided credentials.";
                LoginFormModel = new LoginFormModel();
                ModelState.Clear();

                return Page();
            }
            // enter here if login succeeds
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("id", user.Id.Value.ToString()),
                new Claim("isAdmin", user.IsAdmin.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));
            return new RedirectToPageResult("Index");
        }
    }
}

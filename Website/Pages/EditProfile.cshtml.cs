using System.Security.Claims;
using LogicLayer.Exceptions;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages
{
    [Authorize]
    public class EditProfileModel : PageModel
    {
        private readonly UserManager userManager;

        public EditProfileModel(UserManager userManager)
        {
            this.userManager = userManager;
        }

        [BindProperty]
        public EditProfileFormModel Form { get; set; } = new();

        public bool CanManageSteamId { get; private set; }

        public IActionResult OnGet()
        {
            var user = GetCurrentUser();
            if (user == null)
                return Challenge();

            CanManageSteamId = !user.IsAdmin;
            Form = new EditProfileFormModel
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Age = user.Age,
                Username = user.Username,
                Email = user.Gmail,
                SteamProfile = user.SteamId is null or "0" ? null : user.SteamId
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existingUser = GetCurrentUser();
            if (existingUser == null)
                return Challenge();

            CanManageSteamId = !existingUser.IsAdmin;
            if (!ModelState.IsValid)
                return Page();

            var passwordHash = string.IsNullOrWhiteSpace(Form.NewPassword)
                ? existingUser.Password
                : BCrypt.Net.BCrypt.HashPassword(Form.NewPassword);

            var requestedSteamProfile = CanManageSteamId
                ? string.IsNullOrWhiteSpace(Form.SteamProfile)
                    ? existingUser.SteamId
                    : Form.SteamProfile
                : null;

            var updatedUser = new User(
                existingUser.Id,
                Form.Firstname.Trim(),
                Form.Lastname.Trim(),
                Form.Age.GetValueOrDefault(),
                Form.Username.Trim(),
                Form.Email.Trim(),
                passwordHash,
                existingUser.IsAdmin,
                requestedSteamProfile);

            try
            {
                userManager.UpdateUser(updatedUser);
            }
            catch (UsernameAlreadyInUseException)
            {
                ModelState.AddModelError("Form.Username", "This username is already taken.");
                return Page();
            }
            catch (EmailAlreadyInUseException)
            {
                ModelState.AddModelError("Form.Email", "This email is already registered.");
                return Page();
            }
            catch (InvalidSteamIdException exception)
            {
                ModelState.AddModelError("Form.SteamProfile", exception.Message);
                return Page();
            }
            catch (SteamIdAlreadyInUseException exception)
            {
                ModelState.AddModelError("Form.SteamProfile", exception.Message);
                return Page();
            }

            await RefreshAuthenticationAsync(updatedUser);
            TempData["ProfileMessage"] = "Profile updated successfully.";
            return RedirectToPage("/ViewProfile");
        }

        private User? GetCurrentUser()
        {
            return int.TryParse(User.FindFirst("id")?.Value, out var userId)
                ? userManager.GetUserById(userId)
                : null;
        }

        private async Task RefreshAuthenticationAsync(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new("id", user.Id!.Value.ToString())
            };

            if (user.IsAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }
    }
}

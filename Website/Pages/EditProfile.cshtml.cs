using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using Microsoft.AspNetCore.Authorization;
using LogicLayer.Models;
using System.Diagnostics;

namespace Website.Pages
{
    [Authorize]
    public class EditProfileModel : PageModel
    {
        [BindProperty]
        public FullUserFormModel FullUserFormModel { get; set; } = new();

        private readonly UserManager _userManager;

        public EditProfileModel(UserManager userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            var user = GetCurrentUser();
            if (user == null)
                return Challenge();

            FullUserFormModel = new FullUserFormModel(
                user.Firstname,
                user.Lastname,
                user.Age,
                user.Username,
                user.Gmail,
                ""
            );

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var oldUser = GetCurrentUser();
            if (oldUser == null)
                return Challenge();

            string finalPassword = string.IsNullOrWhiteSpace(FullUserFormModel.Password)
                ? oldUser.Password
                : BCrypt.Net.BCrypt.HashPassword(FullUserFormModel.Password);

            var updated = new User(
                oldUser.Id,
                FullUserFormModel.Firstname,
                FullUserFormModel.Lastname,
                FullUserFormModel.Age.GetValueOrDefault(),
                FullUserFormModel.Username,
                FullUserFormModel.Gmail,
                finalPassword,
                oldUser.IsAdmin,
                oldUser.SteamId
            );

            try
            {
                _userManager.UpdateUser(updated);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                Debug.WriteLine(ex.Message);
                return Page();
            }

            return RedirectToPage("ViewProfile");
        }

        private User? GetCurrentUser()
        {
            return int.TryParse(User.FindFirst("id")?.Value, out var userId)
                ? _userManager.GetUserById(userId)
                : null;
        }
    }
}

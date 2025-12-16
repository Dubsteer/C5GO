using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using Microsoft.AspNetCore.Authorization;
using LogicLayer.Models;
using LogicLayer.Exceptions;
using System.Diagnostics;
using BCrypt.Net;

namespace Website.Pages
{
    [Authorize]
    public class EditProfileModel : PageModel
    {
        [BindProperty]
        public FullUserFormModel FullUserFormModel { get; set; }

        private readonly UserManager _userManager;

        public EditProfileModel(UserManager userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            int userId = int.Parse(User.FindFirst("id").Value);
            var user = _userManager.GetUserById(userId);

            FullUserFormModel = new FullUserFormModel(
                user.Firstname,
                user.Lastname,
                user.Age,
                user.Username,
                user.Gmail,
                "" // password empty
            );

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            int userId = int.Parse(User.FindFirst("id").Value);
            var oldUser = _userManager.GetUserById(userId);

            // Ako je lozinka ostavljena prazna ? ?uvamo staru lozinku
            string finalPassword = string.IsNullOrWhiteSpace(FullUserFormModel.Password)
                ? oldUser.Password
                : BCrypt.Net.BCrypt.HashPassword(FullUserFormModel.Password);

            var updated = new User(
                oldUser.Id,
                FullUserFormModel.Firstname,
                FullUserFormModel.Lastname,
                FullUserFormModel.Age.Value,
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
    }
}

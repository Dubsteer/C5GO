using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using Microsoft.AspNetCore.Authorization;
using LogicLayer.Exceptions;
using LogicLayer.Models;
using System.Diagnostics;

namespace Website.Pages
{
    public class EditProfileModel : PageModel
    {
        [BindProperty]
        public FullUserFormModel FullUserFormModel { get; set; }

        private readonly UserManager userManager;

        public EditProfileModel(UserManager userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult OnGet()
        {
            var user = userManager.GetUserById(Convert.ToInt32(User.FindFirst("id").Value));

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
            if(!ModelState.IsValid) 
                return Page();

            var oldUser = userManager.GetUserById(Convert.ToInt32(User.FindFirst("id").Value));

            var newUser = new User(
                oldUser.Id,
                FullUserFormModel.Firstname,
                FullUserFormModel.Lastname,
                FullUserFormModel.Age,
                FullUserFormModel.Username,
                FullUserFormModel.Gmail,
                FullUserFormModel.Password,
                oldUser.IsAdmin
                );

            try
            {
                userManager.UpdateUser(newUser);
            }
            catch (UsernameAlreadyInUseException ex)
            {
                ViewData["Error"] = ex.Message;
                Debug.WriteLine(ex.Message);
                return Page();
            }

            return new RedirectToPageResult("ViewProfile");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Exceptions;
using System.Diagnostics;

namespace Website.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public FullUserFormModel FullUserFormModel { get; set; }

        private readonly UserManager userManager;

        public RegisterModel(UserManager userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = new User(
                FullUserFormModel.Firstname,
                FullUserFormModel.Lastname,
                FullUserFormModel.Age.Value,
                FullUserFormModel.Username,
                FullUserFormModel.Gmail,
                FullUserFormModel.Password,
                false
            );

            try
            {
                userManager.CreateUser(user);
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }

            return Redirect("/");
        }
    }
}

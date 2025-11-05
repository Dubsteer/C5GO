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
        public List<User> Users { get; set; }
        public User newUser { get; set; }


        public RegisterModel(UserManager userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            else
                return Page();
        }

        public IActionResult OnPost()
        {

            User user = new User();

            if (ModelState.IsValid)
            {

                 user = new User(
                    FullUserFormModel.Firstname,
                    FullUserFormModel.Lastname,
                    FullUserFormModel.Age,
                    FullUserFormModel.Username,
                    FullUserFormModel.Gmail,
                    FullUserFormModel.Password,
                    false
                );

                try
                {
                    userManager.CreateUser(user);
                }
                catch (UsernameAlreadyInUseException ex)
                {
                    ViewData["Error"] = ex.Message;
                    Debug.WriteLine(ex.Message);
                    return Page();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // internal server error
                    return StatusCode(500);
                }
            }

            return Redirect("/");
        }
    }
}

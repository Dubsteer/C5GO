using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Exceptions;
using LogicLayer.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Website.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public FullUserFormModel FullUserFormModel { get; set; }

        private readonly UserManager userManager;
        private readonly EmailService emailService;

        public RegisterModel(UserManager userManager, EmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");

            return Page();
        }

        public async Task<IActionResult> OnPost()
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
                // CREATE USER + TOKEN
                userManager.CreateUser(user);

                // SEND VERIFICATION EMAIL
                await emailService.SendVerificationEmail(
                    user.Gmail,
                    user.EmailToken
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
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }

            return Redirect("/RegisterSuccess");
        }
    }
}

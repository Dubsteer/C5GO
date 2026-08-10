using LogicLayer.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Diagnostics;

namespace Website.Pages
{
    public class VerifyEmailModel : PageModel
    {
        private readonly UserManager userManager;

        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }

        public VerifyEmailModel(UserManager userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult OnGet(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Message = "Invalid verification link.";
                IsSuccess = false;
                return Page();
            }

            try
            {
                userManager.ConfirmEmail(token);
                Message = "Your email has been successfully verified!";
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Message = "This verification link is invalid or has expired.";
                IsSuccess = false;
            }

            return Page();
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace LogicLayer.FormModels
{
    public class ForgotPasswordFormModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;
    }
}

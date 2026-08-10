using System.ComponentModel.DataAnnotations;

namespace LogicLayer.FormModels
{
    public class ResetPasswordFormModel
    {
        [Required(ErrorMessage = "New password is required")]
        [StringLength(72, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 72 characters")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your new password")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

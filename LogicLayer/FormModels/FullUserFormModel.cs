using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LogicLayer.FormModels
{
    public class FullUserFormModel
    {
        [Required(ErrorMessage = "First name is required")]
        [DisplayName("First name")]
        [StringLength(25, ErrorMessage = "First name must not exceed {1} characters")]
        public string Firstname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [DisplayName("Last name")]
        [StringLength(35, ErrorMessage = "Last name must not exceed {1} characters")]
        public string Lastname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        [DisplayName("Date of birth")]
        [DataType(DataType.Date)]
        [AllowedBirthDate]
        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, ErrorMessage = "Username must not exceed {1} characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [DisplayName("Email")]
        public string Gmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(72, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 72 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password")]
        [DisplayName("Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

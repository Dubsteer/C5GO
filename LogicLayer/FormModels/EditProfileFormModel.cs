using System.ComponentModel.DataAnnotations;

namespace LogicLayer.FormModels
{
    public class EditProfileFormModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(25, ErrorMessage = "First name must not exceed {1} characters")]
        public string Firstname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(35, ErrorMessage = "Last name must not exceed {1} characters")]
        public string Lastname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required")]
        [Range(14, 106, ErrorMessage = "Age must be between 14 and 106")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, ErrorMessage = "Username must not exceed {1} characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [StringLength(72, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 72 characters")]
        public string? NewPassword { get; set; }

        [StringLength(100, ErrorMessage = "Steam profile value must not exceed {1} characters")]
        public string? SteamProfile { get; set; }
    }
}

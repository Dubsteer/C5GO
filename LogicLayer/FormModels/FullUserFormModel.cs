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

        [Required(ErrorMessage = "Age is required")]
        [DisplayName("Age")]
        [Range(14, 106, ErrorMessage = "Age must be between 14 and 106")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [DisplayName("Email")]
        public string Gmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(72, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 72 characters")]
        public string Password { get; set; } = string.Empty;

        public FullUserFormModel() { }

        public FullUserFormModel(
            string firstname,
            string lastname,
            int age,
            string username,
            string gmail,
            string password)
        {
            Firstname = firstname;
            Lastname = lastname;
            Age = age;
            Username = username;
            Gmail = gmail;
            Password = password;
        }
    }
}

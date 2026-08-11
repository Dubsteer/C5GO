using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LogicLayer.FormModels
{
    public class LoginFormModel
    {
        [Required]
        [DisplayName("Username")]
        [StringLength(30, ErrorMessage = "Username length must not exceed {1} characters.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DisplayName("Password")]
        [StringLength(72, ErrorMessage = "Password length must not exceed {1} characters.")]
        public string Password { get; set; } = string.Empty;

        public LoginFormModel() { }

        public LoginFormModel(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}

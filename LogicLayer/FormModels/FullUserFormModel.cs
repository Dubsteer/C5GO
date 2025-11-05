using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LogicLayer.FormModels
{
    public class FullUserFormModel: LoginFormModel
    {
        [Required]
        [DisplayName("First name")]
        [StringLength(25, ErrorMessage = "First name length must not exceed {1} characters.")]
        public string Firstname { get; set; }

        [Required]
        [DisplayName("Last name")]
        [StringLength(35, ErrorMessage = "Last name length must not exceed {1} characters.")]
        public string Lastname { get; set; }

        [Required]
        [DisplayName("Age")]
        [Range(14, 106, ErrorMessage = "Age must be between 14 and 106.")]
        public int Age { get; set; }


        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        [StringLength(255, ErrorMessage = "Gmail length must not exceed {1} characters.")]
        public string Gmail { get; set; }

       

        public FullUserFormModel() { }

        public FullUserFormModel(string firstname, string lastname, int age, string username, string gmail, string password) : base(username, password)
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

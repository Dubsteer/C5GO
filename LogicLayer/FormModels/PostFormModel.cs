using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LogicLayer.FormModels
{
    public class PostFormModel
    {
        [Required]
        [DisplayName("Content")]
        [StringLength(int.MaxValue, ErrorMessage = "Content length must not exceed {1} characters.")]
        public string Content { get; set; } = string.Empty;

        public PostFormModel() { }
        public PostFormModel(string content)
        {
            Content = content;
        }
    }
}

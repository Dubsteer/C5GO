using LogicLayer.Enums;
using LogicLayer.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LogicLayer.FormModels
{
    public class TournamentFormModel
    {
        [Required]
        [DisplayName("Name")]
        [StringLength(50, ErrorMessage = "Content length must not exceed {1} characters.")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Description")]
        [StringLength(300, ErrorMessage = "Content length must not exceed {1} characters.")]
        public string Description { get; set; }

        [Display(Name = "Status")]
        [DisplayName("Status")]
        public Status status { get; set; }

        public TournamentFormModel() { }

        public TournamentFormModel(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}

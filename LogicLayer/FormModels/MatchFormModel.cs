using LogicLayer.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LogicLayer.FormModels
{
    public class MatchFormModel
    {
        [Display(Name = "Id")]
        [DisplayName("Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        [DisplayName("Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [DisplayName("Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Status")]
        [DisplayName("Status")]
        public Status Status { get; set; }

        public MatchFormModel(int id, string description, string name)
        {
            Id = id;
            Description = description;
            Name = name;
        }
    }
}

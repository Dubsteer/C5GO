using LogicLayer.Enums;
using LogicLayer.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace LogicLayer.FormModels
{
    public class MatchFormModel
    {
        [Display(Name = "Id")]
        [DisplayName("Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [DisplayName("Description")]
        public string Description { get; set; }

        [Display(Name = "Status")]
        [DisplayName("Status")]
        public Status status { get; set; }

        public MatchFormModel(int Id, string description, string name) 
        {
            Id = Id;
            Description = description;
            Name = name; ;
            
        }
    }
}

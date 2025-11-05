using LogicLayer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using LogicLayer.Enums;

namespace LogicLayer.Models
{
    public class Tournament
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

        [Display(Name = "Closed")]
        [DisplayName("Closed")]
        public bool Closed { get; set; }

        public List<Player> Players { get; set; }
        public List<Match> Matches { get; set; }

        public Tournament(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;

        }

        public Tournament(int id, string name, string description, bool closed)
        {
            Id = id;
            Name = name;
            Description = description;
            Closed = closed;
        }

        public Tournament(object value, string name, string description)
        {
            Name = name;
            Description = description;
            Closed = true;  
        }
    }
}
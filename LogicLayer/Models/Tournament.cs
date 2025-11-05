using LogicLayer.Enums;
using System.Collections.Generic;

namespace LogicLayer.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Status ENUM (Open, InProgress, Closed)
        public Status Status { get; set; }

        // Players & Matches lists
        public List<Player> Players { get; set; } = new();
        public List<Match> Matches { get; set; } = new();

        // Helper properties
        public bool IsOpen => Status == Status.Open;
        public bool IsClosed => Status == Status.Closed;
        public bool IsInProgress => Status == Status.InProgress;

        public Tournament() { }

        public Tournament(int id, string name, string description, Status status)
        {
            Id = id;
            Name = name;
            Description = description;
            Status = status;
        }
    }
}

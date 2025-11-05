using System;

namespace LogicLayer.Exceptions
{
    public class TournamentClosedException : Exception
    {
        public TournamentClosedException() : base() { }

        public TournamentClosedException(string message) : base(message) { }

        public TournamentClosedException(string message, Exception inner) : base(message, inner) { }
    }
}

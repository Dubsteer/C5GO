
namespace LogicLayer.Exceptions
{
    public class TournamentNotFoundException : Exception
    {
        public TournamentNotFoundException() { }

        public TournamentNotFoundException(string message) : base(message) { }

        public TournamentNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}


namespace LogicLayer.Exceptions
{
    public class MatchNotFoundException : Exception
    {
        public MatchNotFoundException() { }

        public MatchNotFoundException(string message) : base(message) { }

        public MatchNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}

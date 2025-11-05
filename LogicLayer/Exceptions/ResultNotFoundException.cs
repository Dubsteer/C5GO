
namespace LogicLayer.Exceptions
{
    public class ResultNotFoundException : Exception
    {
        public ResultNotFoundException() { }

        public ResultNotFoundException(string message) : base(message) { }

        public ResultNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}

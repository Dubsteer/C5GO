namespace LogicLayer.Exceptions
{
    public class EmailAlreadyInUseException : Exception
    {
        public EmailAlreadyInUseException() { }

        public EmailAlreadyInUseException(string message) : base(message) { }

        public EmailAlreadyInUseException(string message, Exception innerException) : base(message, innerException) { }
    }
}

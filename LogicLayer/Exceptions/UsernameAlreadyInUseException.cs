namespace LogicLayer.Exceptions
{
    public class UsernameAlreadyInUseException : Exception
    {
        public UsernameAlreadyInUseException() { }

        public UsernameAlreadyInUseException(string message) : base(message) { }

        public UsernameAlreadyInUseException(string message, Exception innerException) : base(message, innerException) { }
    }
}

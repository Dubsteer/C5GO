namespace LogicLayer.Exceptions
{
    public class PostNameAlreadyInUseExepction : Exception
    {
        public PostNameAlreadyInUseExepction() { }

        public PostNameAlreadyInUseExepction(string message) : base(message) { }

        public PostNameAlreadyInUseExepction (string message, Exception innerException) : base(message, innerException) { }
    }
}

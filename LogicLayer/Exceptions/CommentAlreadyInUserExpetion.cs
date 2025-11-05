namespace LogicLayer.Exceptions
{
    public class CommentAlreadyInUserExpetion : Exception 
    {
        public CommentAlreadyInUserExpetion() { }

        public CommentAlreadyInUserExpetion(string message) : base(message) { }

        public CommentAlreadyInUserExpetion(string message,  Exception innerException) : base(message, innerException) { }
    }
}

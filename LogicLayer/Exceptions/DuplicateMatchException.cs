

namespace LogicLayer.Exceptions
{
    public class DuplicateMatchException : Exception
    {
        public DuplicateMatchException()
        {
        }

        public DuplicateMatchException(string message)
            : base(message)
        {
        }

        public DuplicateMatchException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

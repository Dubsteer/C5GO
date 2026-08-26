namespace LogicLayer.Exceptions
{
    public class InvalidBirthDateException : Exception
    {
        public InvalidBirthDateException()
            : base("You must be between 14 and 106 years old.")
        {
        }
    }
}

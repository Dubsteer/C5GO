namespace LogicLayer.Exceptions
{
    public class InvalidSteamIdException : Exception
    {
        public InvalidSteamIdException()
            : base("Enter a valid 17-digit SteamID64 or Steam profile URL.")
        {
        }
    }
}

namespace LogicLayer.Exceptions
{
    public class SteamIdAlreadyInUseException : Exception
    {
        public SteamIdAlreadyInUseException()
            : base("This Steam ID is already connected to another account.")
        {
        }
    }
}

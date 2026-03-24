namespace LogicLayer.Dtos
{
    public class ExternalStreamDto
    {
        public string Platform { get; set; } = ""; // Twitch, Kick
        public string StreamerName { get; set; } = "";
        public string Url { get; set; } = "";
        public int Viewers { get; set; }
    }
}
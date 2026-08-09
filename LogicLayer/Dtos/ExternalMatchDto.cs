namespace LogicLayer.Dtos
{
    public class ExternalMatchDto
    {
        public string Id { get; set; } = "";

        public string Team1Name { get; set; } = "";
        public string Team2Name { get; set; } = "";

        public string Team1LogoUrl { get; set; } = "";
        public string Team2LogoUrl { get; set; } = "";

        public string EventName { get; set; } = "";

        // može biti null ako API ne pošalje vrijeme
        public DateTime? StartTimeUtc { get; set; }

        // Live / Upcoming / Finished
        public string Status { get; set; } = "";

        // npr "13 - 7"
        public string Score { get; set; } = "";
    }
}

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

        public DateTime? StartTimeUtc { get; set; }

        // Live / Upcoming / Finished
        public string Status { get; set; } = "";

        // npr "13 - 7"
        public string Score { get; set; } = "";
    }
}
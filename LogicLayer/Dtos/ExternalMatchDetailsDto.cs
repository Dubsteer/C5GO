namespace LogicLayer.Dtos
{
    public class ExternalMatchDetailsDto
    {
        public string Id { get; set; } = "";

        public string Team1Name { get; set; } = "";
        public string Team2Name { get; set; } = "";

        public string Team1LogoUrl { get; set; } = "";
        public string Team2LogoUrl { get; set; } = "";

        public string EventName { get; set; } = "";
        public string Format { get; set; } = "";

        public string Status { get; set; } = "";
        public string Score { get; set; } = "";

        public DateTime? StartTimeUtc { get; set; }

        public List<ExternalMapDto> Maps { get; set; } = new();
        public List<ExternalStreamDto> Streams { get; set; } = new();
    }
}

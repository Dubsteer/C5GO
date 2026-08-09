using System.Text.Json.Serialization;

namespace LogicLayer.Dtos
{
    public class PandaMatch
    {
        public long Id { get; set; }

        public string? Status { get; set; }

        [JsonPropertyName("begin_at")]
        public DateTime? BeginAt { get; set; }

        public List<PandaOpponentWrapper> Opponents { get; set; } = new();

        public PandaLeague? League { get; set; }

        public List<PandaResult>? Results { get; set; }

        [JsonPropertyName("streams_list")]
        public List<PandaStream>? StreamsList { get; set; }
    }

    public class PandaOpponentWrapper
    {
        public PandaOpponent? Opponent { get; set; }
    }

    public class PandaOpponent
    {
        public long Id { get; set; }
        public string? Name { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }
    }

    public class PandaLeague
    {
        public string? Name { get; set; }
    }

    public class PandaResult
    {
        [JsonPropertyName("team_id")]
        public long TeamId { get; set; }

        public int Score { get; set; }
    }

    public class PandaStream
    {
        [JsonPropertyName("raw_url")]
        public string? RawUrl { get; set; }
    }
}

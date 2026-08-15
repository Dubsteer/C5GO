namespace Website.Models
{
    public static class BracketRoundPresentation
    {
        public static string GetName(int totalParticipants, int roundNumber, int matchCount)
        {
            if (roundNumber == 1 && matchCount * 2 < totalParticipants)
                return "Preliminary round";

            return matchCount switch
            {
                1 => "Final",
                2 => "Semifinals",
                4 => "Quarterfinals",
                _ => $"Round of {matchCount * 2}"
            };
        }
    }
}

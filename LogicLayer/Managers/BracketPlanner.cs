namespace LogicLayer.Managers
{
    internal static class BracketPlanner
    {
        public static int GetOpeningParticipantCount(int totalParticipants)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(totalParticipants, 2);

            var bracketSize = 1;
            while (bracketSize * 2 <= totalParticipants)
                bracketSize *= 2;

            return bracketSize == totalParticipants
                ? totalParticipants
                : 2 * (totalParticipants - bracketSize);
        }

        public static List<T> Shuffle<T>(IEnumerable<T> participants)
        {
            var result = participants.ToList();

            for (var i = result.Count - 1; i > 0; i--)
            {
                var selected = Random.Shared.Next(i + 1);
                (result[i], result[selected]) = (result[selected], result[i]);
            }

            return result;
        }

        public static List<T> CombineWithByes<T>(IReadOnlyList<T> winners, IReadOnlyList<T> byes)
        {
            var result = new List<T>(winners.Count + byes.Count);
            var winnerIndex = 0;
            var byeIndex = 0;

            while (winnerIndex < winners.Count || byeIndex < byes.Count)
            {
                if (winnerIndex < winners.Count)
                    result.Add(winners[winnerIndex++]);

                if (byeIndex < byes.Count)
                    result.Add(byes[byeIndex++]);
            }

            return result;
        }
    }
}

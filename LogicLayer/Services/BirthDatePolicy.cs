namespace LogicLayer.Services
{
    public static class BirthDatePolicy
    {
        public const int MinimumAge = 14;
        public const int MaximumAge = 106;

        public static bool TryCalculateAllowedAge(
            DateTime birthDate,
            DateTime today,
            out int age)
        {
            age = CalculateAge(birthDate, today);
            return age is >= MinimumAge and <= MaximumAge;
        }

        public static int CalculateAge(DateTime birthDate, DateTime today)
        {
            var normalizedBirthDate = birthDate.Date;
            var normalizedToday = today.Date;
            var age = normalizedToday.Year - normalizedBirthDate.Year;

            if (normalizedBirthDate > normalizedToday.AddYears(-age))
                age--;

            return age;
        }

        public static DateTime GetOldestAllowedBirthDate(DateTime today) =>
            today.Date.AddYears(-(MaximumAge + 1)).AddDays(1);

        public static DateTime GetYoungestAllowedBirthDate(DateTime today) =>
            today.Date.AddYears(-MinimumAge);
    }
}

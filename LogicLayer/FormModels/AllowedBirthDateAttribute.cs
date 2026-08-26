using System.ComponentModel.DataAnnotations;
using LogicLayer.Services;

namespace LogicLayer.FormModels
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AllowedBirthDateAttribute : ValidationAttribute
    {
        public AllowedBirthDateAttribute()
            : base("You must be between 14 and 106 years old")
        {
        }

        public override bool IsValid(object? value)
        {
            return value is not DateTime birthDate ||
                   BirthDatePolicy.TryCalculateAllowedAge(
                       birthDate,
                       DateTime.UtcNow.Date,
                       out _);
        }
    }
}

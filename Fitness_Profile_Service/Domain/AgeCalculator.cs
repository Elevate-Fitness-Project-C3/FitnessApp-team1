namespace FitnessProfileService.Domain;

public static class AgeCalculator
{
    public static int AgeOn(DateOnly dateOfBirth, DateOnly today)
    {
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.AddYears(age) > today) age--;
        return age;
    }
}

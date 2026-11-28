namespace IdentityService.Application;

public static class PasswordPolicy
{
    public static string[] Validate(string? password)
    {
        var errors = new List<string>();
        if (string.IsNullOrEmpty(password)) return ["is required."];
        if (password.Length < 8) errors.Add("must be at least 8 characters.");
        if (!password.Any(char.IsUpper)) errors.Add("must contain an uppercase letter.");
        if (!password.Any(char.IsLower)) errors.Add("must contain a lowercase letter.");
        if (!password.Any(char.IsDigit)) errors.Add("must contain a digit.");
        return errors.ToArray();
    }
}
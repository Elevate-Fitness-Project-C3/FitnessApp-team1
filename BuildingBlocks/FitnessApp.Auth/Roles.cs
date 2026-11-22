namespace FitnessApp.Auth;

public static class Roles
{
    public const string User = "User";
    public const string Admin = "Admin";

    /// <summary>Role assigned to accounts created before ID-01/ID-03 exist.</summary>
    public const string Default = User;
}

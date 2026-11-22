namespace IdentityService.Domain
{
    public class AppUser : IdentityUser
    {
        public bool IsEnabled { get; set; } = true;
    }
}

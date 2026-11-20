
namespace IdentityService.Persistence.Seeding
{
    
        public class RoleSeeder
        {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleSeeder(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
            public  async Task SeedRolesAsync()
            {
                string[] roles = { "User", "Admin" };

                foreach (var role in roles)
                {
                
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(
                            new IdentityRole(role));
                    }
                }
            }
        }
    
}

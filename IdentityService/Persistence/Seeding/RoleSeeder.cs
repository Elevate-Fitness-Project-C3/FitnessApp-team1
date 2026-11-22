//using Microsoft.AspNetCore.Identity;

//namespace IdentityService.Persistence.Seeding;

//public static class RoleSeeder
//{
//    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
//    {
//        string[] roles = { "User", "Admin" };

//        foreach (var role in roles)
//        {
//            if (await roleManager.RoleExistsAsync(role))
//                continue;

//            var result = await roleManager.CreateAsync(new IdentityRole(role));
//            if (!result.Succeeded)
//            {
//                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
//                throw new InvalidOperationException($"Failed to seed role '{role}': {errors}");
//            }
//        }
//    }
//}

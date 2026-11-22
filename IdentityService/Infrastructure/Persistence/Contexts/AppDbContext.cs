

namespace IdentityService.Infrastructure.Persistence.Contexts
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .Property(user => user.IsEnabled)
                .HasDefaultValue(true);
        }
    }
}

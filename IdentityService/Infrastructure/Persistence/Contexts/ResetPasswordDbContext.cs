using IdentityService.Domain;
using IdentityService.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence.Contexts;

public class ResetPasswordDbContext(DbContextOptions<ResetPasswordDbContext> options) : DbContext(options)
{
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfiguration(new PasswordResetTokenConfiguration());

}

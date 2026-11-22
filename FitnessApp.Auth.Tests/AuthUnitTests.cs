using System.Security.Claims;
using FitnessApp.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace FitnessApp.Auth.Tests;

public class AuthUnitTests
{
    private const string ValidKey = "test-signing-key-that-is-definitely-long-enough-1234567890";

    [Fact]
    public void CurrentUser_ParsesSubAndRoleClaims()
    {
        var userId = Guid.NewGuid();
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("sub", userId.ToString()),
                    new Claim("role", Roles.Admin)
                }, "Test", "sub", "role"))
            }
        };

        var current = new CurrentUser(accessor);

        Assert.Equal(userId, current.UserId);
        Assert.True(current.IsAdmin);
    }

    [Fact]
    public void CurrentUser_ThrowsWhenSubMissing()
    {
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(authenticationType: "Test"))
            }
        };

        Assert.Throws<InvalidOperationException>(() => new CurrentUser(accessor).UserId);
    }

    [Fact]
    public void CurrentUser_ThrowsWhenSubIsNotAGuid()
    {
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("sub", "not-a-guid")
                }, "Test"))
            }
        };

        Assert.Throws<InvalidOperationException>(() => new CurrentUser(accessor).UserId);
    }

    [Fact]
    public void JwtOptions_AcceptsKeyOfAtLeast32Characters()
    {
        var options = BuildOptions(ValidKey);

        Assert.Equal("test-issuer", options.Issuer);
        Assert.Equal("test-audience", options.Audience);
        Assert.Equal(ValidKey, options.SigningKey);
    }

    [Fact]
    public void JwtOptions_RejectsShortSigningKey()
    {
        Assert.Throws<OptionsValidationException>(() => BuildOptions("too-short"));
    }

    private static JwtOptions BuildOptions(string key)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "test-issuer",
            ["Jwt:Audience"] = "test-audience",
            ["Jwt:SigningKey"] = key
        }).Build();

        var services = new ServiceCollection();
        services.AddFitnessAuth(config);

        using var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IOptions<JwtOptions>>().Value;
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FitnessApp.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Api;

public sealed record DevTokenRequest(Guid? UserId, string? Role);

public static class DevTokenEndpoints
{
    /// <summary>
    /// TODO: remove this endpoint when ID-03 (login) exists.
    /// It only exists so other services can be exercised locally before login is implemented.
    /// </summary>
    public static IEndpointRouteBuilder MapDevTokenEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/dev/token", (DevTokenRequest req, IOptions<JwtOptions> options) =>
        {
            var role = string.IsNullOrWhiteSpace(req.Role) ? Roles.Default : req.Role.Trim();

            if (role != Roles.User && role != Roles.Admin)
                return Results.BadRequest(new { error = "invalid_role", validRoles = new[] { Roles.User, Roles.Admin } });

            var jwt = options.Value;
            var userId = req.UserId ?? Guid.NewGuid();
            var now = DateTime.UtcNow;

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt.Issuer,
                audience: jwt.Audience,
                claims: new[]
                {
                    new Claim("sub", userId.ToString()),
                    new Claim("role", role)
                },
                notBefore: now,
                expires: now.AddHours(1),
                signingCredentials: credentials);

            return Results.Ok(new
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(token),
                token_type = "Bearer",
                expires_in = 3600,
                userId,
                role
            });
        });

        return app;
    }
}

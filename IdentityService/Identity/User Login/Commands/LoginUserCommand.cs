using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityService.Identity.User_Login.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Identity.User_Login.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<LoginResult>;

public record LoginResult(bool Succeeded, LoginResponse Response);

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResult>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _configuration;

    public LoginUserCommandHandler(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<LoginResult> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return new LoginResult(false, null);
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
             true);
        if (!signInResult.Succeeded || !user.EmailConfirmed || !user.IsEnabled)
        {
            return new LoginResult(false, null);
        }

        var accessToken = await CreateAccessTokenAsync(user, cancellationToken);
        return new LoginResult(true, accessToken);
    }

    private async Task<LoginResponse> CreateAccessTokenAsync(AppUser user,CancellationToken cancellationToken)
    {

        var secret = _configuration["Jwt:Secret"];
        
        var tokenLifetimeInMinutes =
            _configuration.GetValue<int>("Jwt:AccessTokenMinutes");

        var userRoles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var signingCredentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var expirationTime = DateTime.UtcNow.AddMinutes(tokenLifetimeInMinutes);
        var jwtToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims:claims,
            expires: expirationTime,
            signingCredentials: signingCredentials);

        var tokenText = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return new LoginResponse(tokenText, expirationTime);
    }
}

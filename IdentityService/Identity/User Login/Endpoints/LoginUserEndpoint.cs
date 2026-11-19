using FluentValidation;
using IdentityService.Identity.User_Login.Commands;
using IdentityService.Identity.User_Login.ViewModels;

namespace IdentityService.Identity.User_Login.Endpoints;

public static class LoginUserEndpoint
{
    public static void UserLogin(this WebApplication app)
    {
        app.MapPost("/api/Account/login", async (LoginUserViewModel model,
            IValidator<LoginUserViewModel> validator,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var validation = await validator.ValidateAsync(model, cancellationToken);
            if (!validation.IsValid)
            {
                return Results.BadRequest("Invalid credentials");
            }
            var result = await mediator.Send(new LoginUserCommand(model.Email, model.Password),cancellationToken);

            if (!result.Succeeded)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(result.Response);
        })
        .WithTags("Account").RequireRateLimiting("fixed");
    }
}

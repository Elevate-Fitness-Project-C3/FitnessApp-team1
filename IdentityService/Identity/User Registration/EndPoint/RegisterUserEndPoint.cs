using FluentValidation;
using IdentityService.Identity.User_Registration.Commands;
using IdentityService.Identity.User_Registration.ViewModels;

namespace IdentityService.Identity.User_Registration.EndPoint
{
    public static class RegisterUserEndPoint
    {
        public static async Task RegisterUser(this WebApplication app)
        {
            app.MapPost("/api/Account", async (RegisterUserViewModel registerUserViewModel, IValidator<RegisterUserViewModel> validator,IMediator mediator) =>
            {

              var result = await validator.ValidateAsync(registerUserViewModel);
              if (!result.IsValid)
              {
                  return Results.BadRequest("Invalid user data provided.");
              }
             var registrationResult =  await mediator.Send(new RegisterUserCommand(registerUserViewModel.UserName, registerUserViewModel.Email, registerUserViewModel.Password, registerUserViewModel.ConfirmPassword));
                if(registrationResult == false)
                {
                    return Results.BadRequest("User registration failed. Please check the provided information.");
                }
                return Results.Ok("User registered successfully."); 
            }).WithTags("Account").RequireRateLimiting("fixed").AllowAnonymous();
        }
    }
}

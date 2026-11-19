using FluentValidation;
using IdentityService.Identity.User_Login.ViewModels;

namespace IdentityService.Identity.User_Login.Validation;

public sealed class LoginUserViewModelValidator : AbstractValidator<LoginUserViewModel>
{
    public LoginUserViewModelValidator()
    {
        RuleFor(model => model.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(model => model.Password)
            .NotEmpty();
    }
}

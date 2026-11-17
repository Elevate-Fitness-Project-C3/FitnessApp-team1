using FluentValidation;
using IdentityService.Identity.User_Registration.ViewModels;

namespace IdentityService.Identity.User_Registration.Validation
{
    public class RegisterViewModelValidation :AbstractValidator<RegisterUserViewModel>
    {
        public RegisterViewModelValidation() 
        {
            RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);
            RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password)
            .WithMessage("Password and Confirm Password must match.");
        }
        
    }
}

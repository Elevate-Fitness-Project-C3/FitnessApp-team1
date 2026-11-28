using IdentityService.Application.EmailVerification;

namespace IdentityService.Identity.User_Registration.Commands
{
    public record RegisterUserCommand
        (string userName, string email, string password, string confirmPassword) : IRequest<bool>;

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
    {
        private readonly UserManager<AppUser> _userManger;
        private readonly IEmailVerificationService _emailVerificationService;
        public RegisterUserCommandHandler(UserManager<AppUser> userManger, IEmailVerificationService emailVerificationService)
        {
            _userManger = userManger;
            _emailVerificationService = emailVerificationService;
        }
        public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new AppUser
            {
                UserName = request.userName,
                Email = request.email,
                EmailConfirmed = false
            };
            if(await _userManger.FindByNameAsync(user.UserName) != null)
            {
                return false;
            }

            if ( await _userManger.FindByEmailAsync(user.Email) == null)
            {
                var creatingUserResult = await _userManger.CreateAsync(user, request.password);

                if (creatingUserResult.Succeeded)
                {
                   var addingRoleResult = await _userManger.AddToRoleAsync(user, "User");
                    if (addingRoleResult.Succeeded)
                    {
                        if (Guid.TryParse(user.Id, out var userId) && !string.IsNullOrWhiteSpace(user.Email))
                        {
                            await _emailVerificationService.IssueAsync(userId, user.Email, cancellationToken);
                        }
                        return true;
                    }
                    else
                    {
                        
                       await _userManger.DeleteAsync(user);
                       return false;
                        
                    }
                    
                }
                
            }
            return false;
        }
    }

}

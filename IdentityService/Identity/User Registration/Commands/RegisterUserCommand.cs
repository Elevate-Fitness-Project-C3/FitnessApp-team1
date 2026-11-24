namespace IdentityService.Identity.User_Registration.Commands
{
    public record RegisterUserCommand
        (string userName, string email, string password, string confirmPassword) : IRequest<bool>;

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
    {
        private readonly UserManager<AppUser> _userManger;
        public RegisterUserCommandHandler(UserManager<AppUser> userManger)
        {
            _userManger = userManger;
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

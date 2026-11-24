namespace IdentityService.Identity.User_Login.DTOs;

public record LoginResponse(string AccessToken,DateTime ExpiresAt);

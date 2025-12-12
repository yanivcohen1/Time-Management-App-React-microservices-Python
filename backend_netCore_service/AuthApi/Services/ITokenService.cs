using AuthApi.Models;

namespace AuthApi.Services;

public interface ITokenService
{
    AuthResponse CreateToken(ApplicationUser user);
}

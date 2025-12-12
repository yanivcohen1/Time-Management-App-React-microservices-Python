using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthApi.Models;
using AuthApi.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Services;

public class TokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    public AuthResponse CreateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return new AuthResponse(accessToken, user.Username, user.Role, expires);
    }
}

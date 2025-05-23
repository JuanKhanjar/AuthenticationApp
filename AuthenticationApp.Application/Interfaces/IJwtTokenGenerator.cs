using AuthenticationApp.Domain.Entities;

namespace AuthenticationApp.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();
        DateTime GetTokenExpiry();
    }
}

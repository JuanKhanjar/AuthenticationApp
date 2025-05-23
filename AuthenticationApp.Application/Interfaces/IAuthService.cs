using AuthenticationApp.Application.DTOs;

namespace AuthenticationApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken);

        Task<bool> ConfirmEmailAsync(string userId, string token);
    }
}

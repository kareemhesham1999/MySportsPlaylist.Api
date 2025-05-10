using MySportsPlaylist.Api.Models;

namespace MySportsPlaylist.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        string GenerateJwtToken(User user);
    }
}
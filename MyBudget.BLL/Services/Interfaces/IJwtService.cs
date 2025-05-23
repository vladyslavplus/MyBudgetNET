using MyBudget.BLL.DTOs.Auth;

namespace MyBudget.BLL.Services.Interfaces;

public interface IJwtService
{
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    Task<bool> RegisterAsync(string username, string email, string password);
    Task<RefreshTokenResponseDto> RefreshTokenAsync(string ipAddress);
    Task<bool> ConfirmEmailAsync(string userId, string token);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    Task LogoutAsync();
}
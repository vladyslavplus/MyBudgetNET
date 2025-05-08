using MyBudget.BLL.DTOs.Auth;

namespace MyBudget.BLL.Services.Interfaces;

public interface IJwtService
{
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    Task<bool> RegisterAsync(string username, string email, string password);
}
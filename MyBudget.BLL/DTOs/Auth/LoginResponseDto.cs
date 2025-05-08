namespace MyBudget.BLL.DTOs.Auth;

public class LoginResponseDto
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? AccessToken { get; set; }
    public int ExpiresIn { get; set; }
}
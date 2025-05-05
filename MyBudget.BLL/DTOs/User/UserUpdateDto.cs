namespace MyBudget.BLL.DTOs.User;

public class UserUpdateDto
{
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; } 
    public string? Password { get; set; } 
    public bool IsBlocked { get; set; }
}
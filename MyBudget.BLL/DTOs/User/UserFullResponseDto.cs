using MyBudget.BLL.DTOs.Expense;

namespace MyBudget.BLL.DTOs.User;

public class UserFullResponseDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
    public List<ExpenseMiniResponseDto> Expenses { get; set; } = [];
}
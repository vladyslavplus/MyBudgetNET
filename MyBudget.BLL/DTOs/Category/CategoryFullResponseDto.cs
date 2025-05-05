using MyBudget.BLL.DTOs.Expense;

namespace MyBudget.BLL.DTOs.Category;

public class CategoryFullResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ExpenseMiniResponseDto> Expenses { get; set; } = new();
}
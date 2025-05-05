namespace MyBudget.BLL.DTOs.Expense;

public class ExpenseMiniResponseDto
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
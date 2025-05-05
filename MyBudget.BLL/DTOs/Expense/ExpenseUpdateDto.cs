namespace MyBudget.BLL.DTOs.Expense;

public class ExpenseUpdateDto
{
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
}
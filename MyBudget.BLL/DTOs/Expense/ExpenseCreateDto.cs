namespace MyBudget.BLL.DTOs.Expense;

public class ExpenseCreateDto
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int UserId { get; set; }
}
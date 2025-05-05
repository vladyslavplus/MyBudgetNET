namespace MyBudget.DAL.Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsBlocked { get; set; } = false;
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
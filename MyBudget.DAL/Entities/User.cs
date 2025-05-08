using Microsoft.AspNetCore.Identity;

namespace MyBudget.DAL.Entities;

public class User : IdentityUser
{ 
    public bool IsBlocked { get; set; } = false;
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
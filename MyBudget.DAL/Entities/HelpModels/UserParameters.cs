namespace MyBudget.DAL.Entities.HelpModels;

public class UserParameters : QueryStringParameters
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool? IsBlocked { get; set; }
}
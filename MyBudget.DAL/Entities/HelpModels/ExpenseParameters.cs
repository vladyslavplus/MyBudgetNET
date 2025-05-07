namespace MyBudget.DAL.Entities.HelpModels;

public class ExpenseParameters : QueryStringParameters
{
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? UserName { get; set; }
    public string? CategoryName { get; set; }
}
﻿namespace MyBudget.BLL.DTOs.Expense;

public class ExpenseResponseDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public string User { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
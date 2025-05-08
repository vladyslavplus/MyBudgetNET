using FluentValidation;
using MyBudget.BLL.DTOs.Expense;

namespace MyBudget.BLL.Validators.Expense;

public class ExpenseUpdateDtoValidator : AbstractValidator<ExpenseUpdateDto>
{
    public ExpenseUpdateDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description can't exceed 500 characters.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Category is required.");
    }
}
using FluentValidation;
using MyBudget.BLL.DTOs.Category;

namespace MyBudget.BLL.Validators.Category;

public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MinimumLength(4).MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");
    }
}
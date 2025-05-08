using FluentValidation;
using MyBudget.BLL.DTOs.Category;

namespace MyBudget.BLL.Validators.Category;

public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
{
    public CategoryUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category Name is required")
            .MinimumLength(4).MaximumLength(100).WithMessage("Category Name must not exceed 100 characters");
    }
}
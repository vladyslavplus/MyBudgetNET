using FluentValidation;
using MyBudget.BLL.DTOs.User;

namespace MyBudget.BLL.Validators.User;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required.")
            .MinimumLength(6).WithMessage("UserName must be at least 3 characters long.");

        When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Email!)
                .EmailAddress().WithMessage("Email format is invalid.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Password), () =>
        {
            RuleFor(x => x.Password!)
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        });
    }
}
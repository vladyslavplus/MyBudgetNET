using FluentValidation;
using MyBudget.BLL.DTOs.Auth;

namespace MyBudget.BLL.Validators.Auth;

public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenRequestDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .MinimumLength(20).WithMessage("Refresh token must be at least 20 characters long.");
    }
}
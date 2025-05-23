using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MyBudget.BLL.DTOs.Auth;
using MyBudget.BLL.DTOs.Category;
using MyBudget.BLL.DTOs.Expense;
using MyBudget.BLL.DTOs.User;
using MyBudget.BLL.Services;
using MyBudget.BLL.Services.Interfaces;
using MyBudget.BLL.Validators.Auth;
using MyBudget.BLL.Validators.Category;
using MyBudget.BLL.Validators.Expense;
using MyBudget.BLL.Validators.User;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace MyBudget.BLL;

public static class Extensions
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
        
        services.AddFluentValidationAutoValidation();
        services.AddScoped<IValidator<CategoryCreateDto>, CategoryCreateDtoValidator>();
        services.AddScoped<IValidator<CategoryUpdateDto>, CategoryUpdateDtoValidator>();

        services.AddScoped<IValidator<LoginRequestDto>, LoginRequestDtoValidator>();
        services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestDtoValidator>();
        services.AddScoped<IValidator<RefreshTokenRequestDto>, RefreshTokenRequestDtoValidator>();
        
        services.AddScoped<IValidator<ExpenseCreateDto>, ExpenseCreateDtoValidator>();
        services.AddScoped<IValidator<ExpenseUpdateDto>, ExpenseUpdateDtoValidator>();
        
        services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();
        services.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();
        
        services.AddScoped<IValidator<ForgotPasswordRequestDto>, ForgotPasswordRequestDtoValidator>();
        services.AddScoped<IValidator<ResetPasswordRequestDto>, ResetPasswordRequestDtoValidator>();
        
        services.AddScoped<IEmailService, EmailService>();
        
        services.AddHttpContextAccessor();
        
        return services;
    }
}
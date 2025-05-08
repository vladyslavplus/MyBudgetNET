using System.Text;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MyBudget.BLL.Configuration;
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
using MyBudget.DAL.Data;
using MyBudget.DAL.Entities;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace MyBudget.BLL;

public static class Extensions
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        MapsterConfig.RegisterMappings();

        services.AddMapster();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();


        services.AddFluentValidationAutoValidation();
        services.AddScoped<IValidator<CategoryCreateDto>, CategoryCreateDtoValidator>();
        services.AddScoped<IValidator<CategoryUpdateDto>, CategoryUpdateDtoValidator>();

        services.AddScoped<IValidator<LoginRequestDto>, LoginRequestDtoValidator>();
        services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestDtoValidator>();
        
        services.AddScoped<IValidator<ExpenseCreateDto>, ExpenseCreateDtoValidator>();
        services.AddScoped<IValidator<ExpenseUpdateDto>, ExpenseUpdateDtoValidator>();
        
        services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();
        services.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();
        
        return services;
    }

    public static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // IN PROD = TRUE
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["JwtConfig:Issuer"],
                    ValidAudience = configuration["JwtConfig:Audience"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Key"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });

        services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }
}
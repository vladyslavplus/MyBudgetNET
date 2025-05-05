using Mapster;
using Microsoft.Extensions.DependencyInjection;
using MyBudget.BLL.Configuration;
using MyBudget.BLL.Services;
using MyBudget.BLL.Services.Interfaces;

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
        
        return services;
    }
}
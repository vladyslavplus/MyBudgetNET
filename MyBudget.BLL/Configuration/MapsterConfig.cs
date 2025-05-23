using Mapster;
using Microsoft.Extensions.DependencyInjection;
using MyBudget.BLL.DTOs.Expense;
using MyBudget.DAL.Entities;

namespace MyBudget.BLL.Configuration;

public static class MapsterConfig
{
    private static void RegisterMappings()
    {
        TypeAdapterConfig<Expense, ExpenseMiniResponseDto>.NewConfig()
            .Map(dest => dest.Category, src => src.Category.Name);
        
        TypeAdapterConfig<Expense, ExpenseResponseDto>.NewConfig()
            .Map(dest => dest.Category, src => src.Category.Name)
            .Map(dest => dest.User, src => src.User.UserName);
    }
    
    public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
    {
        MapsterConfig.RegisterMappings();
        services.AddMapster();

        return services;
    }
}
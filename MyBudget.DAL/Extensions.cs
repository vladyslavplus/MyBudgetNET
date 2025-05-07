using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBudget.DAL.Data;
using MyBudget.DAL.Entities;
using MyBudget.DAL.Helpers;
using MyBudget.DAL.UOW;

namespace MyBudget.DAL;

public static class Extensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ISortHelper<Category>, SortHelper<Category>>();
        services.AddScoped<ISortHelper<Expense>, SortHelper<Expense>>();
        services.AddScoped<ISortHelper<User>, SortHelper<User>>();        
        
        return services;
    }
}
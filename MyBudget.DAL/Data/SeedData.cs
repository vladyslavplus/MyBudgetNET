using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBudget.DAL.Entities;

namespace MyBudget.DAL.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        string[] roles = ["Admin", "User"];
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        if (!context.Users.Any())
        {
            var userFaker = new Faker<User>()
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.IsBlocked, f => f.Random.Bool());

            const string password = "TestPassword123";
            var users = userFaker.Generate(10);

            foreach (var user in users)
            {
                user.EmailConfirmed = true;
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
                else
                {
                    throw new Exception($"Failed to seed user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            var admin = new User
            {
                UserName = "admin123",
                Email = "admin@mybudget.com",
                EmailConfirmed = true
            };

            var adminResult = await userManager.CreateAsync(admin, "Admin@1234");
            if (adminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        var usedNames = new HashSet<string>();
        var categoryFaker = new Faker<Category>()
            .RuleFor(c => c.Name, f =>
            {
                string name;
                do
                {
                    name = f.Commerce.Categories(1).First();
                } while (!usedNames.Add(name));
                return name;
            });

        var categories = categoryFaker.Generate(5);
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        var allUsers = await userManager.Users.ToListAsync();
        var allCategories = context.Categories.ToList();

        var expenseFaker = new Faker<Expense>()
            .RuleFor((Expense e) => e.Amount, f => f.Finance.Amount())
            .RuleFor(e => e.Date, f => f.Date.Recent().ToUniversalTime())
            .RuleFor(e => e.Description, f => f.Lorem.Sentence())
            .RuleFor(e => e.UserId, f => f.PickRandom(allUsers).Id)
            .RuleFor(e => e.CategoryId, f => f.PickRandom(allCategories).Id);

        var expenses = expenseFaker.Generate(20);
        context.Expenses.AddRange(expenses);
        await context.SaveChangesAsync();
    }
}

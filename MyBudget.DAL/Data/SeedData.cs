using Bogus;
using MyBudget.DAL.Entities;

namespace MyBudget.DAL.Data;

public static class SeedData
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Users.Any())
            return; 

        var userFaker = new Faker<User>()
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => f.Internet.Password())
            .RuleFor(u => u.IsBlocked, f => f.Random.Bool());

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

        var users = userFaker.Generate(10);
        context.Users.AddRange(users);

        var categories = categoryFaker.Generate(5);
        context.Categories.AddRange(categories);

        context.SaveChanges();

        var allUsers = context.Users.ToList();
        var allCategories = context.Categories.ToList();

        var expenseFaker = new Faker<Expense>()
            .RuleFor(e => e.Amount, f => f.Finance.Amount())
            .RuleFor(e => e.Date, f => f.Date.Recent().ToUniversalTime()) 
            .RuleFor(e => e.Description, f => f.Lorem.Sentence())
            .RuleFor(e => e.UserId, f => f.PickRandom(allUsers).Id)
            .RuleFor(e => e.CategoryId, f => f.PickRandom(allCategories).Id);
 

        var expenses = expenseFaker.Generate(20);
        context.Expenses.AddRange(expenses);

        context.SaveChanges(); 
    }
}
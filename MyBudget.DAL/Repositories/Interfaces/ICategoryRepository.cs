using MyBudget.DAL.Entities;

namespace MyBudget.DAL.Repositories.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetAllWithExpensesAsync(CancellationToken cancellationToken = default);
}
using MyBudget.DAL.Entities;
using MyBudget.DAL.Entities.HelpModels;
using MyBudget.DAL.Helpers;

namespace MyBudget.DAL.Repositories.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetAllWithExpensesAsync(CancellationToken cancellationToken = default);
    Task<PagedList<Category>> GetAllPaginatedAsync(CategoryParameters parameters, ISortHelper<Category> sortHelper, CancellationToken cancellationToken = default);
}
using MyBudget.DAL.Entities;
using MyBudget.DAL.Entities.HelpModels;
using MyBudget.DAL.Helpers;

namespace MyBudget.DAL.Repositories.Interfaces;

public interface IExpenseRepository : IGenericRepository<Expense>
{
    Task<IEnumerable<Expense>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Expense>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<PagedList<Expense>> GetAllPaginatedAsync(ExpenseParameters parameters, ISortHelper<Expense> sortHelper, CancellationToken cancellationToken = default);
    Task<Expense?> GetByIdWithCategoryAndUserAsync(int id, CancellationToken cancellationToken = default);
}
using MyBudget.DAL.Entities;

namespace MyBudget.DAL.Repositories.Interfaces;

public interface IExpenseRepository : IGenericRepository<Expense>
{
    Task<Expense?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Expense>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Expense>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
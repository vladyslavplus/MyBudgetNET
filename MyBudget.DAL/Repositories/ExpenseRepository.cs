using Microsoft.EntityFrameworkCore;
using MyBudget.DAL.Data;
using MyBudget.DAL.Entities;
using MyBudget.DAL.Repositories.Interfaces;

namespace MyBudget.DAL.Repositories;

public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
{
    public ExpenseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Expense>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await ExpensesWithCategory
            .Where(e => e.UserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Expense>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await ExpensesWithCategory
            .Where(e => e.CategoryId == categoryId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Expense>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await ExpensesWithCategory
            .Where(e => e.Date >= startDate && e.Date <= endDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Expense?> GetByIdWithCategoryAndUserAsync(int id, CancellationToken cancellationToken = default)
    {
        return await ExpensesWithCategory
            .Include(e => e.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    
    private IQueryable<Expense> ExpensesWithCategory => _dbSet.Include(e => e.Category);
}
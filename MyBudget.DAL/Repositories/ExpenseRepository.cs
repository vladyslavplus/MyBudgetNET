using Microsoft.EntityFrameworkCore;
using MyBudget.DAL.Data;
using MyBudget.DAL.Entities;
using MyBudget.DAL.Entities.HelpModels;
using MyBudget.DAL.Helpers;
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

    public async Task<PagedList<Expense>> GetAllPaginatedAsync(ExpenseParameters parameters, ISortHelper<Expense> sortHelper, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(e => e.Category)
            .Include(e => e.User)
            .AsQueryable();

        if (parameters.MinAmount is not null)
            query = query.Where(e => e.Amount >= parameters.MinAmount);

        if (parameters.MaxAmount is not null)
            query = query.Where(e => e.Amount <= parameters.MaxAmount);

        if (parameters.DateFrom is not null)
            query = query.Where(e => e.Date >= parameters.DateFrom);

        if (parameters.DateTo is not null)
            query = query.Where(e => e.Date <= parameters.DateTo);

        if (!string.IsNullOrWhiteSpace(parameters.CategoryName))
            query = query.Where(e => e.Category.Name.ToLower().Contains(parameters.CategoryName.ToLower()));

        if (!string.IsNullOrWhiteSpace(parameters.UserName))
            query = query.Where(e => e.User.UserName.ToLower().Contains(parameters.UserName.ToLower()));

        query = sortHelper.ApplySort(query, parameters.OrderBy);

        return await PagedList<Expense>.ToPagedListAsync(
            query.AsNoTracking(),
            parameters.PageNumber,
            parameters.PageSize,
            cancellationToken
        );
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
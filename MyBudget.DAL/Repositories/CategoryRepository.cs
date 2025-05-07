using Microsoft.EntityFrameworkCore;
using MyBudget.DAL.Data;
using MyBudget.DAL.Entities;
using MyBudget.DAL.Entities.HelpModels;
using MyBudget.DAL.Helpers;
using MyBudget.DAL.Repositories.Interfaces;

namespace MyBudget.DAL.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAllWithExpensesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.Expenses)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedList<Category>> GetAllPaginatedAsync(CategoryParameters parameters, ISortHelper<Category> sortHelper, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Name))
            query = query.Where(c => c.Name.ToLower().Contains(parameters.Name.ToLower()));

        query = sortHelper.ApplySort(query, parameters.OrderBy);

        return await PagedList<Category>.ToPagedListAsync(
            query.AsNoTracking(),
            parameters.PageNumber,
            parameters.PageSize,
            cancellationToken
        );
    }
}
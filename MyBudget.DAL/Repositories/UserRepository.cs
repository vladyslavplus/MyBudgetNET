using Microsoft.EntityFrameworkCore;
using MyBudget.DAL.Data;
using MyBudget.DAL.Entities;
using MyBudget.DAL.Entities.HelpModels;
using MyBudget.DAL.Helpers;
using MyBudget.DAL.Repositories.Interfaces;

namespace MyBudget.DAL.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<User>> GetAllUsersWithExpensesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(u => u.Expenses)
            .ThenInclude(e => e.Category)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(u => u.Expenses)
            .ThenInclude(e => e.Category)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<PagedList<User>> GetAllPaginatedAsync(UserParameters parameters, ISortHelper<User> sortHelper, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(parameters.UserName))
            query = query.Where(e => e.UserName.Contains(parameters.UserName));

        if (!string.IsNullOrEmpty(parameters.Email))
            query = query.Where(e => e.Email.Contains(parameters.Email));

        if (parameters.IsBlocked is not null)
            query = query.Where(e => e.IsBlocked == parameters.IsBlocked);

        query = sortHelper.ApplySort(query, parameters.OrderBy);

        return await PagedList<User>.ToPagedListAsync(
            query.AsNoTracking(),
            parameters.PageNumber,
            parameters.PageSize,
            cancellationToken
        );
    }

    public async Task<User?> GetUserWithExpensesAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(u => u.Expenses)
            .ThenInclude(e => e.Category)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> IsBlockedAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.IsBlocked)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
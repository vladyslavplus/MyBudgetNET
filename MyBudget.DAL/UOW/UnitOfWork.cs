using MyBudget.DAL.Data;
using MyBudget.DAL.Repositories;
using MyBudget.DAL.Repositories.Interfaces;

namespace MyBudget.DAL.UOW;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IUserRepository Users { get; }
    public IExpenseRepository Expenses { get; }
    public ICategoryRepository Categories { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        
        Users = new UserRepository(context);
        Expenses = new ExpenseRepository(context);
        Categories = new CategoryRepository(context);
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}
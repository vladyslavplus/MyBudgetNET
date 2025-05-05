using MyBudget.DAL.Data;
using MyBudget.DAL.Repositories;
using MyBudget.DAL.Repositories.Interfaces;

namespace MyBudget.DAL.UOW;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IUserRepository UserRepository { get; }
    public IExpenseRepository ExpenseRepository { get; }
    public ICategoryRepository CategoryRepository { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        
        UserRepository = new UserRepository(context);
        ExpenseRepository = new ExpenseRepository(context);
        CategoryRepository = new CategoryRepository(context);
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
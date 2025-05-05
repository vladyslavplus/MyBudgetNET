using MyBudget.DAL.Repositories.Interfaces;

namespace MyBudget.DAL.UOW;

public interface IUnitOfWork : IDisposable
{
    IUserRepository UserRepository { get; }
    IExpenseRepository ExpenseRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
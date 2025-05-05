using MyBudget.DAL.Repositories.Interfaces;

namespace MyBudget.DAL.UOW;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IExpenseRepository Expenses { get; }
    ICategoryRepository Categories { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
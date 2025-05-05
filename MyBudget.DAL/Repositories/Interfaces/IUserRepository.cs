using MyBudget.DAL.Entities;

namespace MyBudget.DAL.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<IEnumerable<User>> GetAllUsersWithExpensesAsync(CancellationToken cancellationToken = default);
    Task<User?> GetUserWithExpensesAsync(int userId, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsBlockedAsync(int userId, CancellationToken cancellationToken = default);
}
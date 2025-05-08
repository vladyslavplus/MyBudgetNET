using MyBudget.BLL.DTOs.Expense;
using MyBudget.DAL.Entities.HelpModels;
using MyBudget.DAL.Helpers;

namespace MyBudget.BLL.Services.Interfaces;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseMiniResponseDto>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<ExpenseResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExpenseMiniResponseDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<PagedList<ExpenseResponseDto>> GetPaginatedAsync(ExpenseParameters parameters, CancellationToken cancellationToken = default);
    Task<ExpenseResponseDto> CreateAsync(ExpenseCreateDto dto, CancellationToken cancellationToken = default);
    Task<ExpenseResponseDto> UpdateAsync(int id, ExpenseUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
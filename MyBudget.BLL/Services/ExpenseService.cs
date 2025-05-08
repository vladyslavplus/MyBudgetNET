using Mapster;
using MyBudget.BLL.DTOs.Expense;
using MyBudget.BLL.Exceptions;
using MyBudget.BLL.Services.Interfaces;
using MyBudget.DAL.Entities;
using MyBudget.DAL.Entities.HelpModels;
using MyBudget.DAL.Helpers;
using MyBudget.DAL.UOW;

namespace MyBudget.BLL.Services;

public class ExpenseService : IExpenseService
{
    private readonly IUnitOfWork _unitOfWork;

    public ExpenseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ExpenseMiniResponseDto>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var expenses = await _unitOfWork.Expenses.GetByUserIdAsync(userId, cancellationToken);
        return expenses.Adapt<IEnumerable<ExpenseMiniResponseDto>>();
    }

    public async Task<ExpenseResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var expense = await _unitOfWork.Expenses.GetByIdWithCategoryAndUserAsync(id, cancellationToken)
                      ?? throw new NotFoundException($"Expense with ID {id} not found.");

        return expense.Adapt<ExpenseResponseDto>();
    }
    
    public async Task<IEnumerable<ExpenseMiniResponseDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var expenses = await _unitOfWork.Expenses.GetByCategoryIdAsync(categoryId, cancellationToken);
        return expenses.Adapt<IEnumerable<ExpenseMiniResponseDto>>();
    }

    public async Task<PagedList<ExpenseResponseDto>> GetPaginatedAsync(ExpenseParameters parameters, CancellationToken cancellationToken = default)
    {
        var pagedExpenses = await _unitOfWork.Expenses
            .GetAllPaginatedAsync(parameters, new SortHelper<Expense>(), cancellationToken);

        var mapped = pagedExpenses.Select(e => e.Adapt<ExpenseResponseDto>()).ToList();

        return new PagedList<ExpenseResponseDto>(
            mapped,
            pagedExpenses.TotalCount,
            pagedExpenses.CurrentPage,
            pagedExpenses.PageSize
        );
    }

    public async Task<ExpenseResponseDto> CreateAsync(ExpenseCreateDto dto, CancellationToken cancellationToken = default)
    {
        var expense = dto.Adapt<Expense>();
        await _unitOfWork.Expenses.CreateAsync(expense, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
            
        var createdExpense = await _unitOfWork.Expenses
            .GetByIdWithCategoryAndUserAsync(expense.Id, cancellationToken);
        
        return createdExpense.Adapt<ExpenseResponseDto>();
    }

    public async Task<ExpenseResponseDto> UpdateAsync(int id, ExpenseUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var expense = await GetExistingExpenseOrThrowAsync(id, cancellationToken);

        dto.Adapt(expense);
        _unitOfWork.Expenses.Update(expense, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedExpense = await _unitOfWork.Expenses
            .GetByIdWithCategoryAndUserAsync(expense.Id, cancellationToken);

        return updatedExpense.Adapt<ExpenseResponseDto>();

    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var expense = await GetExistingExpenseOrThrowAsync(id, cancellationToken);

        _unitOfWork.Expenses.Delete(expense, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
    
    private async Task<Expense> GetExistingExpenseOrThrowAsync(int id, CancellationToken cancellationToken)
    {
        var expense = await _unitOfWork.Expenses.GetByIdAsync(id, cancellationToken);
        if (expense is null)
            throw new NotFoundException($"Expense with ID {id} not found.");
        return expense;
    }
}
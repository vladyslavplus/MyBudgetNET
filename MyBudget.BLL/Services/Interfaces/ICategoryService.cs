using MyBudget.BLL.DTOs.Category;
using MyBudget.DAL.Entities;
using MyBudget.DAL.Entities.HelpModels;
using MyBudget.DAL.Helpers;

namespace MyBudget.BLL.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryFullResponseDto>> GetAllWithExpensesAsync(CancellationToken cancellationToken = default);
    Task<PagedList<CategoryResponseDto>> GetPaginatedAsync(CategoryParameters parameters, CancellationToken cancellationToken = default);
    Task<CategoryResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto, CancellationToken cancellationToken = default);
    Task<CategoryResponseDto> UpdateAsync(int id, CategoryUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
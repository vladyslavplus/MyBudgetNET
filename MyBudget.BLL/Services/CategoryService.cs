using System.ComponentModel.DataAnnotations;
using Mapster;
using MyBudget.BLL.DTOs.Category;
using MyBudget.BLL.Exceptions;
using MyBudget.BLL.Services.Interfaces;
using MyBudget.DAL.Entities;
using MyBudget.DAL.Entities.HelpModels;
using MyBudget.DAL.Helpers;
using MyBudget.DAL.UOW;

namespace MyBudget.BLL.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
        return categories.Adapt<IEnumerable<CategoryResponseDto>>();
    }

    public async Task<IEnumerable<CategoryFullResponseDto>> GetAllWithExpensesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Categories.GetAllWithExpensesAsync(cancellationToken);
        return categories.Adapt<IEnumerable<CategoryFullResponseDto>>();
    }

    public async Task<PagedList<CategoryResponseDto>> GetPaginatedAsync(CategoryParameters parameters, CancellationToken cancellationToken = default)
    {
        var pagedCategories = await _unitOfWork.Categories
            .GetAllPaginatedAsync(parameters, new SortHelper<Category>(), cancellationToken);

        var mapped = pagedCategories.Select(c => c.Adapt<CategoryResponseDto>()).ToList();

        return new PagedList<CategoryResponseDto>(
            mapped,
            pagedCategories.TotalCount,
            pagedCategories.CurrentPage,
            pagedCategories.PageSize
        );
    }

    public async Task<CategoryResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await GetExistingCategoryOrThrowAsync(id, cancellationToken);
        return category.Adapt<CategoryResponseDto>();
    }
    
    public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ValidationException("Category name cannot be empty.");

        var existing = await _unitOfWork.Categories.GetByNameAsync(dto.Name, cancellationToken);
        if (existing is not null)
            throw new ConflictException($"Category with name '{dto.Name}' already exists.");

        var category = dto.Adapt<Category>();
        await _unitOfWork.Categories.CreateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Adapt<CategoryResponseDto>();
    }

    public async Task<CategoryResponseDto> UpdateAsync(int id, CategoryUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ValidationException("Category name cannot be empty.");

        var category = await GetExistingCategoryOrThrowAsync(id, cancellationToken);

        var existing = await _unitOfWork.Categories.GetByNameAsync(dto.Name, cancellationToken);
        if (existing is not null && existing.Id != id)
            throw new ConflictException($"Another category with name '{dto.Name}' already exists.");

        category.Name = dto.Name;
        _unitOfWork.Categories.Update(category, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return category.Adapt<CategoryResponseDto>();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await GetExistingCategoryOrThrowAsync(id, cancellationToken);

        _unitOfWork.Categories.Delete(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
    
    private async Task<Category> GetExistingCategoryOrThrowAsync(int id, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new NotFoundException($"Category with ID {id} not found.");
        return category;
    }
}
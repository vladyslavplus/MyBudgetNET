using System.ComponentModel.DataAnnotations;
using FakeItEasy;
using FluentAssertions;
using Mapster;
using MyBudget.BLL.DTOs.Category;
using MyBudget.BLL.DTOs.Expense;
using MyBudget.BLL.Exceptions;
using MyBudget.BLL.Services;
using MyBudget.BLL.Services.Interfaces;
using MyBudget.DAL.Entities;
using MyBudget.DAL.Entities.HelpModels;
using MyBudget.DAL.Helpers;
using MyBudget.DAL.Repositories.Interfaces;
using MyBudget.DAL.UOW;

namespace MyBudget.BLL.Tests;

public class CategoryServiceTests
{
    private readonly CancellationToken _ct = CancellationToken.None;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryService _categoryService;

    public CategoryServiceTests()
    {
        _unitOfWork = A.Fake<IUnitOfWork>();
        _categoryRepository = A.Fake<ICategoryRepository>();
        
        A.CallTo(() => _unitOfWork.Categories).Returns(_categoryRepository);
        
        _categoryService = new CategoryService(_unitOfWork);
        
        TypeAdapterConfig<Expense, ExpenseMiniResponseDto>.NewConfig()
            .Map(dest => dest.Category, src => src.Category.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedCategoryResponseDtoList()
    {
        var fakeCategories = GetFakeCategories();
        
        A.CallTo(() => _categoryRepository.GetAllAsync(_ct))
            .Returns(fakeCategories);
        
        var result = await _categoryService.GetAllAsync(_ct);

        var categoryResponseDtos = result as CategoryResponseDto[] ?? result.ToArray();
        categoryResponseDtos.Should().NotBeNull();
        categoryResponseDtos.Should().HaveCount(2);
        categoryResponseDtos.Should().AllBeOfType<CategoryResponseDto>();
        
        var first = categoryResponseDtos.Should().ContainSingle(c => c.Name == "Food").Subject;
        first.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetAllWithExpensesAsync_ShouldReturnMappedCategoryFullResponseDtoList()
    {
        var fakeCategories = GetFakeCategoriesWithExpenses();

        A.CallTo(() => _categoryRepository.GetAllWithExpensesAsync(_ct))
            .Returns(fakeCategories);
        
        var result = await _categoryService.GetAllWithExpensesAsync(_ct);

        var categoryFullResponseDtos = result.ToList();
        categoryFullResponseDtos.Should().NotBeNull();
        categoryFullResponseDtos.Should().HaveCount(2);
        categoryFullResponseDtos.Should().AllBeOfType<CategoryFullResponseDto>();
        
        var utilities = categoryFullResponseDtos.First(c => c.Name == "Utilities");
        utilities.Expenses.Should().HaveCount(2);
        utilities.Expenses.Should().Contain(e => e.Amount == 120.50m && e.Date == new DateTime(2024, 12, 1));
        utilities.Expenses.Should().Contain(e => e.Amount == 80.00m && e.Date == new DateTime(2024, 12, 5));
        utilities.Expenses.Should().AllSatisfy(e => e.Category.Should().Be("Utilities"));

        var groceries = categoryFullResponseDtos.First(c => c.Name == "Groceries");
        groceries.Expenses.Should().ContainSingle(e =>
            e.Amount == 45.99m &&
            e.Date == new DateTime(2024, 12, 3) &&
            e.Category == "Groceries");
    }

    [Fact]
    public async Task GetPaginatedAsync_ShouldReturnPagedCategoryResponseDtoList()
    {
        var parameters = new CategoryParameters
        {
            PageNumber = 1,
            PageSize = 2,
            Name = "Food"  
        };

        var categories = GetFakeCategories();
        
        var pagedCategories = new PagedList<Category>(
            items: categories,
            count: 5,
            pageNumber: 1,
            pageSize: 2
        );
        
        A.CallTo(() => _categoryRepository.GetAllPaginatedAsync(parameters, A<ISortHelper<Category>>._, _ct))
            .Returns(pagedCategories);
        
        var result = await _categoryService.GetPaginatedAsync(parameters, _ct);
        
        result.Should().NotBeNull();
        result.Should().BeOfType<PagedList<CategoryResponseDto>>();
        result.TotalCount.Should().Be(5);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalPages.Should().Be(3);
        result.Count.Should().Be(2); 
        result.Should().ContainSingle(c => c.Name == "Food" && c.Id == 1);
        result.Should().ContainSingle(c => c.Name == "Transport" && c.Id == 2);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategoryResponseDto_WhenCategoryExists()
    {
        const int categoryId = 1;
        var category = new Category{Id = categoryId, Name = "Test Category"};
        var expectedResponseDto = category.Adapt<CategoryResponseDto>();
        
        A.CallTo(() => _categoryRepository.GetByIdAsync(categoryId, _ct))!
            .Returns(Task.FromResult(category));
        
        var result = await _categoryService.GetByIdAsync(categoryId, _ct);
        
        result.Should().BeEquivalentTo(expectedResponseDto);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        const int categoryId = 1;
        
        A.CallTo(() => _categoryRepository.GetByIdAsync(categoryId, _ct))!
            .Returns(Task.FromResult<Category>(null!));
        
        await Assert.ThrowsAsync<NotFoundException>(() => _categoryService.GetByIdAsync(categoryId, _ct));
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCategoryResponseDto_WhenValid()
    {
        var dto = new CategoryCreateDto { Name = "Health" };

        A.CallTo(() => _categoryRepository.GetByNameAsync(dto.Name, _ct))
            .Returns(Task.FromResult<Category?>(null));

        var fakeCategory = new Category { Id = 1, Name = dto.Name };

        A.CallTo(() => _categoryRepository.CreateAsync(A<Category>._, _ct))
            .Returns(Task.FromResult(fakeCategory));

        A.CallTo(() => _unitOfWork.SaveChangesAsync(_ct))
            .Returns(Task.FromResult(1));

        var result = await _categoryService.CreateAsync(dto, _ct);

        result.Should().NotBeNull();
        result.Name.Should().Be("Health");

        A.CallTo(() => _categoryRepository.CreateAsync(
                A<Category>.That.Matches(c => c.Name == dto.Name), _ct))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_ShouldThrowValidationException_WhenNameIsEmpty(string? invalidName)
    {
        var dto = new CategoryCreateDto { Name = invalidName };

        Func<Task> act = () => _categoryService.CreateAsync(dto, _ct);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Category name cannot be empty.");
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrowConflictException_WhenCategoryWithSameNameExists()
    {
        var dto = new CategoryCreateDto { Name = "Food" };

        A.CallTo(() => _categoryRepository.GetByNameAsync(dto.Name, _ct))
            .Returns(new Category { Id = 1, Name = "Food" });

        Func<Task> act = () => _categoryService.CreateAsync(dto, _ct);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Category with name 'Food' already exists.");
    }


    [Fact]
    public async Task UpdateAsync_ShouldUpdateAndReturnCategoryResponseDto_WhenValid()
    {
        var dto = new CategoryUpdateDto { Name = "Updated Health" };
        var category = new Category { Id = 1, Name = "Health" };
        
        A.CallTo(() => _categoryRepository.GetByIdAsync(category.Id, _ct))
            .Returns(category);
        
        A.CallTo(() => _categoryRepository.GetByNameAsync(dto.Name, _ct))
            .Returns<Category?>(null);

        A.CallTo(() => _unitOfWork.SaveChangesAsync(_ct))
            .Returns(Task.FromResult(1));
        
        var result = await _categoryService.UpdateAsync(1, dto, _ct);
        
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Updated Health");
        
        A.CallTo(() => _unitOfWork.SaveChangesAsync(_ct)).MustHaveHappenedOnceExactly();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateAsync_ShouldThrowValidationException_WhenNameIsEmpty(string? invalidName)
    {
        var dto = new CategoryUpdateDto { Name = invalidName };

        Func<Task> act = () => _categoryService.UpdateAsync(1, dto, _ct);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Category name cannot be empty.");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenCategoryNotFound()
    {
        var dto = new CategoryUpdateDto { Name = "Updated Health" };
        
        A.CallTo(() => _categoryRepository.GetByIdAsync(1, _ct))    
            .Returns<Category?>(null);  
        
        Func<Task> act = () => _categoryService.UpdateAsync(1, dto, _ct);
        
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Category with id 1 not found.");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowConflictException_WhenCategoryWithSameNameExists()
    {
        var dto = new CategoryUpdateDto { Name = "Duplicate Name" };
        var category = new Category { Id = 1, Name = "Original" };
        var existing = new Category { Id = 2, Name = "Duplicate Name" };
        
        A.CallTo(() => _categoryRepository.GetByIdAsync(category.Id, _ct))
            .Returns(category);

        A.CallTo(() => _categoryRepository.GetByNameAsync(dto.Name, _ct))
            .Returns(existing);
        
        Func<Task> act = () => _categoryService.UpdateAsync(1, dto, _ct);
        
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Another category with name 'Duplicate Name' already exists.");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteCategory_WhenCategoryExists()
    {
        var category = new Category { Id = 1, Name = "Test" };

        A.CallTo(() => _categoryRepository.GetByIdAsync(category.Id, _ct))
            .Returns(category);

        A.CallTo(() => _categoryRepository.Delete(category, _ct))
            .DoesNothing();

        A.CallTo(() => _unitOfWork.SaveChangesAsync(_ct))
            .Returns(1);

        await _categoryService.DeleteAsync(category.Id, _ct);

        A.CallTo(() => _categoryRepository.Delete(category, _ct))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _unitOfWork.SaveChangesAsync(_ct))
            .MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        const int nonExistentCategoryId = 99999;

        A.CallTo(() => _categoryRepository.GetByIdAsync(nonExistentCategoryId, _ct))
            .Returns(Task.FromResult<Category?>(null));

        Func<Task> act = () => _categoryService.DeleteAsync(nonExistentCategoryId, _ct);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category with ID {nonExistentCategoryId} not found.");

        A.CallTo(() => _categoryRepository.Delete(A<Category>._, _ct))
            .MustNotHaveHappened();

        A.CallTo(() => _unitOfWork.SaveChangesAsync(_ct))
            .MustNotHaveHappened();
    }
    
    private static List<Category> GetFakeCategories()
    {
        return
        [
            new Category { Id = 1, Name = "Food" },
            new Category { Id = 2, Name = "Transport" }
        ];
    }
    
    private static List<Category> GetFakeCategoriesWithExpenses()
    {
        return
        [
            new Category
            {
                Id = 1,
                Name = "Utilities",
                Expenses = new List<Expense>
                {
                    new Expense
                    {
                        Id = 101, Category = new Category { Id = 1, Name = "Utilities" }, Amount = 120.50m,
                        Date = new DateTime(2024, 12, 1)
                    },
                    new Expense
                    {
                        Id = 102, Category = new Category { Id = 1, Name = "Utilities" }, Amount = 80.00m,
                        Date = new DateTime(2024, 12, 5)
                    }
                }
            },

            new Category
            {
                Id = 2,
                Name = "Groceries",
                Expenses = new List<Expense>
                {
                    new Expense
                    {
                        Id = 201, Category = new Category { Id = 2, Name = "Groceries" }, Amount = 45.99m,
                        Date = new DateTime(2024, 12, 3)
                    }
                }
            }
        ];
    }
}
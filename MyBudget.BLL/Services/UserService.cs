using Mapster;
using MyBudget.BLL.DTOs.User;
using MyBudget.BLL.Exceptions;
using MyBudget.BLL.Services.Interfaces;
using MyBudget.DAL.Entities;
using MyBudget.DAL.UOW;

namespace MyBudget.BLL.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<UserFullResponseDto>> GetAllFullAsync(CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Users.GetAllUsersWithExpensesAsync(cancellationToken); 
        return users.Adapt<IEnumerable<UserFullResponseDto>>();
    }

    public async Task<UserFullResponseDto> GetFullByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetUserWithExpensesAsync(id, cancellationToken);
        if (user is null)
            throw new NotFoundException($"User with ID {id} not found.");

        return user.Adapt<UserFullResponseDto>();
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        return users.Adapt<IEnumerable<UserResponseDto>>();
    }

    public async Task<UserResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByIdOrThrowAsync(id, cancellationToken);
        return user.Adapt<UserResponseDto>();
    }

    public async Task<UserResponseDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default)
    {
        await ValidateEmailUniquenessAsync(dto.Email, cancellationToken);
            
        var user = dto.Adapt<User>();
        await _unitOfWork.Users.CreateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Adapt<UserResponseDto>();
    }

    public async Task<UserResponseDto> UpdateAsync(int id, UserUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByIdOrThrowAsync(id, cancellationToken);

        dto.Adapt(user);
        _unitOfWork.Users.Update(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Adapt<UserResponseDto>();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByIdOrThrowAsync(id, cancellationToken);

        _unitOfWork.Users.Delete(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
    
    private async Task<User> GetUserByIdOrThrowAsync(int id, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null)
            throw new NotFoundException($"User with ID {id} not found.");
            
        return user;
    }
    
    private async Task ValidateEmailUniquenessAsync(string email, CancellationToken cancellationToken)
    {
        var emailExists = await _unitOfWork.Users.ExistsByEmailAsync(email, cancellationToken);
        if (emailExists)
            throw new ConflictException($"User with email '{email}' already exists.");
    }
}
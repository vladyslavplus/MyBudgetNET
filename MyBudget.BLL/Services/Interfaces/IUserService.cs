using MyBudget.BLL.DTOs.User;
using MyBudget.DAL.Entities.HelpModels;
using MyBudget.DAL.Helpers;

namespace MyBudget.BLL.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserFullResponseDto>> GetAllFullAsync(CancellationToken cancellationToken = default);
    Task<PagedList<UserResponseDto>> GetPaginatedAsync(UserParameters parameters, CancellationToken cancellationToken = default);
    Task<UserFullResponseDto> GetFullByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserResponseDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default);
    Task<UserResponseDto> UpdateAsync(int id, UserUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
using PTN.InventoryTracking.Application.DTOs.Auth;

namespace PTN.InventoryTracking.Application.Abstractions.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<CurrentUserDto?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

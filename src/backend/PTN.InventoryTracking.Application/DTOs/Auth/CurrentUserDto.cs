namespace PTN.InventoryTracking.Application.DTOs.Auth;

public sealed record CurrentUserDto(
    Guid Id,
    string Email,
    string UserName,
    string FullName,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);

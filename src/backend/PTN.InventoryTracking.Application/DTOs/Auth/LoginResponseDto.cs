namespace PTN.InventoryTracking.Application.DTOs.Auth;

public sealed record LoginResponseDto(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAtUtc,
    CurrentUserDto User);

using System.ComponentModel.DataAnnotations;

namespace PTN.InventoryTracking.Application.DTOs.Auth;

public sealed record LoginRequestDto
{
    [Required, EmailAddress, StringLength(200)]
    public string Email { get; init; } = string.Empty;

    [Required, StringLength(200, MinimumLength = 6)]
    public string Password { get; init; } = string.Empty;
}

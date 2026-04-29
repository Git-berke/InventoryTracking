using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Auth;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(IAuthService authService) : ApiControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        return result is null
            ? UnauthorizedResponse("Invalid email or password.")
            : OkResponse(result, "Login successful.");
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken = default)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return UnauthorizedResponse();
        }

        var result = await authService.GetCurrentUserAsync(userId, cancellationToken);
        return result is null ? NotFoundResponse("Authenticated user could not be found.") : OkResponse(result);
    }
}

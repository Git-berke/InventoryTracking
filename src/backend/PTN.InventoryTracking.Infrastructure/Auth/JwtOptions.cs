namespace PTN.InventoryTracking.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "PTN.InventoryTracking";
    public string Audience { get; set; } = "PTN.InventoryTracking.Client";
    public string SigningKey { get; set; } = "change-this-development-signing-key-1234567890";
    public int ExpirationMinutes { get; set; } = 120;
}

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.Security;
using PTN.InventoryTracking.Infrastructure.Auth;
using PTN.InventoryTracking.Infrastructure.Caching;
using PTN.InventoryTracking.Infrastructure.Realtime;

namespace PTN.InventoryTracking.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var cacheOptions = configuration.GetSection(CacheOptions.SectionName).Get<CacheOptions>() ?? new CacheOptions();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));

        if (cacheOptions.UseRedis && !string.IsNullOrWhiteSpace(cacheOptions.RedisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheOptions.RedisConnectionString;
                options.InstanceName = "ptn-inventory:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrWhiteSpace(accessToken) &&
                            path.StartsWithSegments(InventoryEventsHub.Route))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        var authorizationBuilder = services.AddAuthorizationBuilder();
        authorizationBuilder.SetFallbackPolicy(new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build());

        foreach (var permission in PermissionNames.All)
        {
            authorizationBuilder.AddPolicy(permission, policy =>
                policy.RequireClaim(ClaimNames.Permission, permission));
        }

        services.AddScoped<IAuthService, JwtAuthService>();
        services.AddScoped<IProductStockSummaryCacheService, RedisProductStockSummaryCacheService>();
        services.AddScoped<IRealtimeNotificationService, SignalRRealtimeNotificationService>();
        services.AddSignalR();

        return services;
    }
}

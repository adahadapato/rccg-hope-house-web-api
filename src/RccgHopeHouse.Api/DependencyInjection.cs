using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using RccgHopeHouse.Api.Middleware;
using RccgHopeHouse.Infrastructure;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace RccgHopeHouse.Api;

/// <summary>
/// Top-level composition root for the API layer.
/// Cascades into Infrastructure (which cascades into Application), then adds
/// everything specific to hosting this as an ASP.NET Core Minimal API:
/// exception handling, JSON options, JWT auth, authorization policies,
/// CORS, rate limiting, and Swagger.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Compose Infrastructure (which composes Application)
        services.AddInfrastructure(configuration);

        // 2. Core API Services
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // 3. JSON Configuration
        // Allows enums (ContactReason, PostCategory, PrayerRequestStatus, etc.)
        // to be sent/received as string names ("Membership") instead of raw integers.
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        // 4. Swagger / OpenAPI
        services.AddSwaggerGen(c =>
        {
            //c.OperationFilter<GalleryUploadOperationFilter>();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "RCCG Hope House API",
                Version = "v1",
                Description = "API for church management",
                Contact = new OpenApiContact
                {
                    Email = "enobong.adahada@gmail.com",
                    Name = "Enobong Adahada",
                    Url = new Uri("https://wwww.zayun.biz")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                }
            });

            // Adds the "Authorize" button to Swagger UI and attaches a
            // Bearer token to every request once authorized.
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                    "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                    "Example: \"Bearer 12345abcdef\"",
            });

            c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", null),
                    new List<string>()
                }
            });
        });

        // 5. Rate Limiting
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext => RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions { PermitLimit = 60, Window = TimeSpan.FromMinutes(1) }));

            rateLimiterOptions.AddPolicy("Strict", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(5) }));

            rateLimiterOptions.OnRejected = async (ctx, token) =>
            {
                ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                ctx.HttpContext.Response.ContentType = "application/problem+json";
                await ctx.HttpContext.Response.WriteAsJsonAsync(new
                {
                    type = "https://tools.ietf.org/html/rfc6585#section-4",
                    title = "Too Many Requests",
                    status = 429,
                    detail = "Rate limit exceeded. Please slow down.",
                    instance = ctx.HttpContext.Request.Path
                }, cancellationToken: token);
            };
        });

        // 6. Authentication (JWT)
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var authHeader = context.Request.Headers["Authorization"].ToString();
                        // 🔴 BREAKPOINT HERE — inspect `authHeader` in the debugger.
                        // Empty/missing → Swagger genuinely isn't sending the header.
                        // Present but malformed → formatting issue (double "Bearer", etc).
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        // 🔴 BREAKPOINT HERE — if this fires, the token was received
                        // AND successfully validated. Inspect context.Principal.Claims.
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        // 🔴 BREAKPOINT HERE — inspect context.Exception.Message for
                        // the exact validation failure reason (bad signature, wrong
                        // issuer/audience, expired, etc.)
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        // 🔴 BREAKPOINT HERE — this is what actually produces the
                        // 401 response. context.AuthenticateFailure (if set) often
                        // repeats the failure reason.
                        return Task.CompletedTask;
                    }
                };
            });

        // 7. Authorization Policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
            options.AddPolicy("RequireContentEditor", p => p.RequireRole("Admin", "ContentEditor"));
            options.AddPolicy("RequireMediaManager", p => p.RequireRole("Admin", "MediaManager"));
            options.AddPolicy("RequirePrayerTeam", p => p.RequireRole("Admin", "PrayerTeam"));
        });

        // 8. CORS (Web + Mobile)
        services.AddCors(options =>
        {
            options.AddPolicy("AllowClients", policy =>
                policy.WithOrigins(
                // Production website
                "https://rccghopehouse.org.uk",

                // Temporary SmarterASP.NET frontend URL
                // Remove this when the permanent domain is fully in use.
                "https://adahadapato-003-site1.dtempurl.com",

                // Local web development
                "http://localhost:3000",
                "http://localhost:5173",

                // Mobile / Capacitor development
                "capacitor://localhost",
                "http://localhost:8080")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        // 9. Distributed Cache (used by rate limiter / other infra)
        services.AddDistributedMemoryCache();

        return services;
    }
}

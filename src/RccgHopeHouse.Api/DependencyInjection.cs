
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using RccgHopeHouse.Api.Middleware;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Infrastructure;
using RccgHopeHouse.Worker;

using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace RccgHopeHouse.Api;

/// <summary>
/// Top-level composition root for the API layer.
/// </summary>
/// <remarks>
/// Registers Infrastructure, Application and Worker services,
/// together with API-specific services including exception handling,
/// JSON serialisation, JWT authentication, authorisation,
/// CORS, rate limiting and Swagger.
/// </remarks>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all API services and application dependencies.
    /// </summary>
    /// <param name="services">
    /// The application's service collection.
    /// </param>
    /// <param name="configuration">
    /// The application's configuration.
    /// </param>
    /// <returns>
    /// The configured service collection.
    /// </returns>
    public static IServiceCollection AddApi(this IServiceCollection services,  IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // ====================================================
        // 1. INFRASTRUCTURE, APPLICATION AND WORKER
        // ====================================================

        services.AddInfrastructure(configuration);
        services.AddWorker();

        // ====================================================
        // 2. CORE API SERVICES
        // ====================================================

        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // ====================================================
        // 3. JSON CONFIGURATION
        // ====================================================

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(
                new JsonStringEnumConverter());
        });

        // ====================================================
        // 4. SWAGGER / OPENAPI
        // ====================================================

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "RCCG Hope House API",
                Version = "v1",
                Description = "API for church management",
                Contact = new OpenApiContact
                {
                    Email = "enobong.adahada@gmail.com",
                    Name = "Enobong Adahada",
                    Url = new Uri("https://www.zayun.net")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri(
                        "https://en.wikipedia.org/wiki/MIT_License")
                }
            });

            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "JWT Authorization header using the Bearer scheme. " +
                        "Enter 'Bearer' followed by your JWT token."
                });

            c.AddSecurityRequirement(
                doc => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference(
                            "Bearer",
                            null),
                        new List<string>()
                    }
                });
        });

        // ====================================================
        // 5. RATE LIMITING
        // ====================================================

        services.AddRateLimiter(options =>
        {
            // ------------------------------------------------
            // GLOBAL LIMIT
            // ------------------------------------------------
            // Applies to all endpoints.
            // Maximum: 60 requests per minute per IP address.

            options.GlobalLimiter =
                PartitionedRateLimiter.Create<HttpContext, string>(
                    httpContext =>
                        RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey:
                                httpContext.Connection
                                    .RemoteIpAddress?
                                    .ToString() ?? "unknown",
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 60,
                                Window = TimeSpan.FromMinutes(1),
                                QueueLimit = 0,
                                AutoReplenishment = true
                            }));

            // ------------------------------------------------
            // EXISTING STRICT POLICY
            // ------------------------------------------------
            // Preserves the existing limit of
            // 5 requests every 5 minutes per IP address.

            options.AddPolicy(
                "Strict",
                httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey:
                            httpContext.Connection
                                .RemoteIpAddress?
                                .ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(5),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }));

            // ------------------------------------------------
            // CELEBRATION INVITATION READ POLICY
            // ------------------------------------------------
            // Applies to public GET invitation requests.
            //
            // Maximum: 20 requests per minute per IP address.
            //
            // Helps reduce automated attempts to access
            // invitation details.

            options.AddPolicy("CelebrationInvitationRead",  httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey:
                            httpContext.Connection
                                .RemoteIpAddress?
                                .ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }));

            // ------------------------------------------------
            // CELEBRATION INVITATION RESPONSE POLICY
            // ------------------------------------------------
            // Applies to public POST invitation responses.
            //
            // Maximum: 5 requests per minute per IP address.
            //
            // Protects approval and decline submissions
            // from excessive automated requests.

            options.AddPolicy("CelebrationInvitationRespond",  httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey:
                            httpContext.Connection
                                .RemoteIpAddress?
                                .ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }));

            // ------------------------------------------------
            // RATE LIMIT REJECTION RESPONSE
            // ------------------------------------------------

            options.OnRejected = async (context, ct) =>
            {
                var response = context.HttpContext.Response;

                response.StatusCode =  StatusCodes.Status429TooManyRequests;

                response.ContentType = "application/problem+json";

                await response.WriteAsJsonAsync(
                    new
                    {
                        type = "https://tools.ietf.org/html/rfc6585#section-4",
                        title = "Too Many Requests",
                        status = 429,
                        detail = "Rate limit exceeded. Please slow down.",
                        instance = context.HttpContext.Request.Path
                    },
                    cancellationToken: ct);
            };
        });

        // ====================================================
        // 6. AUTHENTICATION (JWT)
        // ====================================================

        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer =  configuration[AppSettings.Jwt.Issuer],
                        ValidAudience = configuration[AppSettings.Jwt.Audience],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                                    configuration[AppSettings.Jwt.Key]!))
                    };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var authHeader = context.Request.Headers["Authorization"].ToString();

                        return Task.CompletedTask;
                    },

                    OnTokenValidated = context =>
                    {
                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });

        // ====================================================
        // 7. AUTHORISATION POLICIES
        // ====================================================

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.RequireAdmin, policy => policy.RequireRole(Roles.Admin));
            options.AddPolicy(Policies.RequireContentEditor, policy => policy.RequireRole(Roles.Admin, Roles.ContentEditor));
            options.AddPolicy(Policies.RequireMediaManager, policy => policy.RequireRole(Roles.Admin, Roles.MediaManager));
            options.AddPolicy(Policies.RequireUserManager, policy => policy.RequireRole(Roles.Admin, Roles.UserManager));
            options.AddPolicy(Policies.RequirePrayerTeam, policy => policy.RequireRole(Roles.Admin, Roles.PrayerTeam));
        });

        // ====================================================
        // 8. CORS (WEB + MOBILE)
        // ====================================================

        // Retrieve allowed origins from application configuration.
        var allowedOrigins = configuration
            .GetSection(AppSettings.Cors.AllowedOrigins)
            .Get<string[]>()
            ?? throw new InvalidOperationException(
                "CORS allowed origins are not configured.");

        if (allowedOrigins.Length == 0 ||  allowedOrigins.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidOperationException("CORS allowed origins must contain valid entries.");
        }

        services.AddCors(options =>
        {
            options.AddPolicy("AllowClients", policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        // ====================================================
        // 9. DISTRIBUTED CACHE
        // ====================================================

        services.AddDistributedMemoryCache();

        return services;
    }
}

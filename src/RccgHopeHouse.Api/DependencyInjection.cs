using Microsoft.IdentityModel.Tokens;
using RccgHopeHouse.Api.Middleware;
using System.Text;

namespace RccgHopeHouse.Api
{
    public static class DependencyInjection
    {
            public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
            {
            // Register API services here (e.g., controllers, validators, etc.)
            // services.AddScoped<IMyService, MyService>();

            

            // 2. Core Services
            services.AddEndpointsApiExplorer();
            services.AddOpenApi();
            services.AddHttpContextAccessor();
            services.AddTransient<GlobalExceptionHandler>();


            // 3. Auth & JWT
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new()
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
                });

            services.AddAuthorization();

            // 4. CORS (Web + Mobile)
            services.AddCors(options =>
            {
                options.AddPolicy("AllowClients", p =>
                    p.WithOrigins(
                        "https://rccghopehouse.org.uk",
                        "http://localhost:3000",
                        "http://localhost:5173",
                        "capacitor://localhost"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            return services;
        }
    }
}

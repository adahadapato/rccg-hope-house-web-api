using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

//using Microsoft.OpenApi.Models;
using RccgHopeHouse.Api;
using RccgHopeHouse.Api.Endpoints.Auth;
using RccgHopeHouse.Api.Endpoints.ChurchServices;
using RccgHopeHouse.Api.Endpoints.ContactUs;
using RccgHopeHouse.Api.Endpoints.Gallery;
using RccgHopeHouse.Api.Endpoints.PastorPosts;
using RccgHopeHouse.Api.Endpoints.PrayerRequests;
using RccgHopeHouse.Api.Endpoints.Sermons;
using RccgHopeHouse.Api.Endpoints.Thanksgiving;
using RccgHopeHouse.Api.Middleware;
using RccgHopeHouse.Infrastructure;
//using Swashbuckle.AspNetCore.Versioning;

//using Scalar.AspNetCore; // Optional: if you prefer Scalar UI over Swagger
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ==================== Framework Services ====================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHttpContextAccessor();

// ==================== API Versioning ====================
//builder.Services.AddApiVersioning(options =>
//{
//    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
//    options.AssumeDefaultVersionWhenUnspecified = true;
//    options.ReportApiVersions = true;
//    options.ApiVersionReader = Asp.Versioning.ApiVersionReader.Combine(
//        new Asp.Versioning.UrlSegmentApiVersionReader(),
//        new Asp.Versioning.HeaderApiVersionReader("X-API-Version"));
//})
//.AddApiExplorer(options =>
//{
//    options.GroupNameFormat = "'v'VVV";
//    options.SubstituteApiVersionInUrl = true;
//});

// ==================== Swashbuckle + Versioning Integration ====================
// Uses IConfigureOptions pattern to avoid BuildServiceProvider warnings
//builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(c=>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RCCG Hope House API",
        Version = "v1",
        Description = "API for church management",

        Contact = new OpenApiContact()
        {
            Email = "enobong.adahada@gmail.com",
            Name = "Enobong Adahada",
            Url = new Uri("https://wwww.zayun.biz")
        },

        License = new OpenApiLicense()
        {
            Name = "MIT License",
            Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
        }
    });
    //c.OperationFilter<SwaggerDefaultValues>();
}); // Core Swashbuckle registration

// ==================== Rate Limiting ====================
builder.Services.AddRateLimiter(rateLimiterOptions =>
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

// ==================== Authentication & Authorization ====================
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
    options.AddPolicy("RequireContentEditor", p => p.RequireRole("Admin", "ContentEditor"));
    options.AddPolicy("RequireMediaManager", p => p.RequireRole("Admin", "MediaManager"));
    options.AddPolicy("RequirePrayerTeam", p => p.RequireRole("Admin", "PrayerTeam"));
});

// ==================== CORS ====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClients", policy =>
        policy.WithOrigins(
                "https://rccghopehouse.org.uk",
                "http://localhost:3000",
                "http://localhost:5173",
                "capacitor://localhost",
                "http://localhost:8080")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddDistributedMemoryCache();

// ==================== Compose Application + Infrastructure ====================
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// ==================== Middleware Pipeline ====================
if (app.Environment.IsDevelopment())
{
    // Swashbuckle JSON endpoints: /swagger/v1/swagger.json, /swagger/v2/swagger.json
    //app.UseSwagger();

    //// Classic Swagger UI with version dropdown
    //app.UseSwaggerUI(options =>
    //{
    //    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    //    foreach (var desc in provider.ApiVersionDescriptions)
    //    {
    //        options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", $"RCCG Hope House API {desc.ApiVersion}");
    //    }
    //    options.RoutePrefix = "swagger"; // UI at /swagger
    //    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Collapsed by default
    //});

    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.InjectJavascript("https://code.jquery.com/jquery-3.6.0.min.js");
        c.SwaggerEndpoint("v1/swagger.json", "RCCG Hope House API v1");
        c.RoutePrefix = "swagger"; // UI at /swagger
        //options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", $"RCCG Hope House API {desc.ApiVersion}");
        //c.RoutePrefix = "";
        app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
    });

    // Optional: Scalar UI as alternative (uncomment if preferred)
    // app.MapScalarApiReference(options =>
    // {
    //     options.Title = "RCCG Hope House API Docs";
    //     options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.HttpClient, ScalarClient.Fetch);
    //     options.Servers = new[] { new ScalarServer { Url = $"https://localhost:{app.GetPort()}" } };
    // });
}
else
{
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseExceptionHandler();
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseCors("AllowClients");
//app.UseAuthentication();
//app.UseAuthorization();

// ==================== Endpoint Registration ====================
var api = app.MapGroup("/api");
             //.HasApiVersion(1, 0) // ✅ Fixed: Was .HasApiVersion()
             //.WithTags("RCCG Hope House API");

api.MapAuthEndpoints();
api.MapPastorPostEndpoints();
api.MapGalleryEndpoints();
api.MapPrayerRequestEndpoints();
api.MapChurchServiceEndpoints();
api.MapContactUsEndpoints();
api.MapThanksgivingEndpoints();
api.MapSermonEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .ExcludeFromDescription();

app.Run();
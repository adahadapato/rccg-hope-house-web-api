using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Api;
using RccgHopeHouse.Api.Endpoints.Auth;
using RccgHopeHouse.Api.Endpoints.Bibles;
using RccgHopeHouse.Api.Endpoints.ChurchInfo;
using RccgHopeHouse.Api.Endpoints.ChurchServices;
using RccgHopeHouse.Api.Endpoints.ContactUs;
using RccgHopeHouse.Api.Endpoints.Gallery;
using RccgHopeHouse.Api.Endpoints.GalleryCategories;
using RccgHopeHouse.Api.Endpoints.GivingTypes;
using RccgHopeHouse.Api.Endpoints.Members;
using RccgHopeHouse.Api.Endpoints.Offerings;
using RccgHopeHouse.Api.Endpoints.PastorPosts;
using RccgHopeHouse.Api.Endpoints.PrayerRequests;
using RccgHopeHouse.Api.Endpoints.Prophecies;
using RccgHopeHouse.Api.Endpoints.Sermons;
using RccgHopeHouse.Api.Endpoints.ServiceBroadcasts;
using RccgHopeHouse.Api.Endpoints.ThemesOfTheYear;
using RccgHopeHouse.Infrastructure.Identity;
using RccgHopeHouse.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ==================== Gallery File Storage ====================
//
// Use ASP.NET Core's actual web-root path rather than relying on
// AppContext.BaseDirectory.
//
// This ensures that local development writes gallery files into:
//
// src/RccgHopeHouse.Api/wwwroot/uploads/gallery
//
// and that published deployments use the web root belonging to
// the deployed API application.
builder.Configuration["GalleryStorage:WebRootPath"] =
    builder.Environment.WebRootPath;

// ==================== Single Composition Root ====================
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

// ==================== Middleware Pipeline ====================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.InjectJavascript(
            "https://code.jquery.com/jquery-3.6.0.min.js");

        c.SwaggerEndpoint(
            "v1/swagger.json",
            "RCCG Hope House API v1");

        c.RoutePrefix = "swagger";
    });

    app.MapGet(
            "/",
            () => Results.Redirect("/swagger"))
        .ExcludeFromDescription();
}
else
{
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseRateLimiter();

app.UseHttpsRedirection();

// ==================== Static Files ====================
//
// Gallery photographs and generated thumbnails are stored beneath:
//
// wwwroot/uploads/gallery
//
// UseStaticFiles exposes files beneath wwwroot using their
// application-relative paths, for example:
//
// /uploads/gallery/2026/09/abc123.webp
//
// The gallery API stores only these paths and image metadata
// in SQL Server.
app.UseStaticFiles();

app.UseCors("AllowClients");

app.UseAuthentication();

app.UseAuthorization();

// ==================== Endpoint Registration ====================
var api =
    app.MapGroup("/api");

api.MapAuthEndpoints();
api.MapPastorPostEndpoints();
api.MapGalleryEndpoints();
api.MapGalleryCategoryEndpoints();
api.MapPrayerRequestEndpoints();
api.MapChurchServiceEndpoints();
api.MapContactUsEndpoints();
api.MapSermonEndpoints();
api.MapChurchInfoEndpoints();
api.MapMemberEndpoints();
api.MapServiceBroadcastEndpoints();
api.MapGivingTypeEndpoints();
api.MapOfferingEndpoints();
api.MapApiBibleEndpoints();
api.MapProphecyEndpoints();
api.MapThemeOfTheYearEndpoints();

app.MapGet(
        "/health",
        () => Results.Ok(
            new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow
            }))
    .ExcludeFromDescription();

// ==================== Database Seeding ====================
using (var scope = app.Services.CreateScope())
{
    var db =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    var userManager =
        scope.ServiceProvider
            .GetRequiredService<
                UserManager<ApplicationUser>>();

    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<
                RoleManager<IdentityRole>>();

    // Applies any pending EF Core migrations.
    await db.Database.MigrateAsync();

    await RccgHopeHouse.Infrastructure.Persistence.Seeding
        .DbSeeder.SeedAsync(
            db,
            userManager,
            roleManager,
            app.Configuration);
}

app.Run();
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Api;
using RccgHopeHouse.Api.Endpoints.Auth;
using RccgHopeHouse.Api.Endpoints.ChurchInfo;
using RccgHopeHouse.Api.Endpoints.ChurchServices;
using RccgHopeHouse.Api.Endpoints.ContactUs;
using RccgHopeHouse.Api.Endpoints.Gallery;
using RccgHopeHouse.Api.Endpoints.Members;
using RccgHopeHouse.Api.Endpoints.PastorPosts;
using RccgHopeHouse.Api.Endpoints.PrayerRequests;
using RccgHopeHouse.Api.Endpoints.Sermons;
using RccgHopeHouse.Api.Endpoints.ServiceBroadcasts;
using RccgHopeHouse.Infrastructure.Identity;
using RccgHopeHouse.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

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
        c.InjectJavascript("https://code.jquery.com/jquery-3.6.0.min.js");
        c.SwaggerEndpoint("v1/swagger.json", "RCCG Hope House API v1");
        c.RoutePrefix = "swagger";
    });

    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
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
app.UseAuthentication();
app.UseAuthorization();

// ==================== Endpoint Registration ====================
var api = app.MapGroup("/api");

api.MapAuthEndpoints();
api.MapPastorPostEndpoints();
api.MapGalleryEndpoints();
api.MapPrayerRequestEndpoints();
api.MapChurchServiceEndpoints();
api.MapContactUsEndpoints();
api.MapSermonEndpoints();
api.MapChurchInfoEndpoints();
api.MapMemberEndpoints();
api.MapServiceBroadcastEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .ExcludeFromDescription();

// ==================== Database Seeding ====================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await db.Database.MigrateAsync();           // applies any pending migrations
    await RccgHopeHouse.Infrastructure.Persistence.Seeding.DbSeeder.SeedAsync(
        db, userManager, roleManager, app.Configuration);
}

app.Run();
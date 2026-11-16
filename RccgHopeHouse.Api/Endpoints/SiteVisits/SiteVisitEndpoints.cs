using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.SiteVisits;
using RccgHopeHouse.Application.Features.SiteVisits.Commands;
using RccgHopeHouse.Application.Features.SiteVisits.Dtos;
using RccgHopeHouse.Application.Features.SiteVisits.Queries;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Api.Endpoints.SiteVisits;

/// <summary>
/// The website's own visitor counter, shown in "Website Overview"
/// on the admin dashboard. It stores no cookie and no network
/// address: see <see cref="Core.Entities.SiteVisit"/>.
/// </summary>
public static partial class SiteVisitEndpoints
{
    public static RouteGroupBuilder MapSiteVisitEndpoints(
        this RouteGroupBuilder group)
    {
        var siteVisits = group
            .MapGroup("/site-visits")
            .WithTags("Site Visits");

        // ===== Public (called by the website itself) =====

        siteVisits.MapPost("/", RecordAsync)
            .WithName("RecordSiteVisit")
            .WithSummary("Count a page view, or add reading time")
            .WithDescription(
                "Send an empty body (or seconds = null) to count a page view. " +
                "Send seconds to add reading time for the same visitor.")
            .Produces(StatusCodes.Status204NoContent)
            .AllowAnonymous();

        // ===== Admin (any signed-in user: it feeds the dashboard) =====

        siteVisits.MapGet("/admin/overview", GetOverviewAsync)
            .WithName("GetSiteOverview")
            .WithSummary("Website figures for the admin dashboard")
            .Produces<SiteOverviewDto>(StatusCodes.Status200OK)
            .RequireAuthorization();

        return group;
    }

    private static async Task<IResult> RecordAsync(
        [FromBody] RecordSiteVisitRequest? request,
        HttpContext httpContext,
        [FromServices] IMediator mediator,
        [FromServices] IConfiguration configuration,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();

        // Search engines and other robots are not visitors.
        if (string.IsNullOrWhiteSpace(userAgent) ||
            RobotPattern().IsMatch(userAgent))
        {
            return TypedResults.NoContent();
        }

        try
        {
            var visitDate = SiteVisitClock.Today();

            await mediator.Send(
                new RecordSiteVisitCommand(
                    VisitDate: visitDate,
                    VisitorHash: CreateVisitorHash(
                        httpContext,
                        userAgent,
                        visitDate,
                        configuration),
                    IsMobile: MobilePattern().IsMatch(userAgent),
                    Seconds: request?.Seconds),
                ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Counting visits must never get in a visitor's way.
            loggerFactory
                .CreateLogger("SiteVisits")
                .LogWarning(ex, "A website visit could not be counted.");
        }

        return TypedResults.NoContent();
    }

    private static async Task<IResult> GetOverviewAsync(
        [FromServices] IMediator mediator,
        CancellationToken ct,
        [FromQuery] int days = 30)
    {
        return TypedResults.Ok(
            await mediator.Send(new GetSiteOverviewQuery(days), ct));
    }

    /// <summary>
    /// Makes the one-way code that tells one visitor from another.
    /// The date is part of it, so the same person gets a new code
    /// each day, and the secret key means it cannot be worked back
    /// to a network address. The address itself is never stored.
    /// </summary>
    private static string CreateVisitorHash(
        HttpContext httpContext,
        string userAgent,
        DateOnly visitDate,
        IConfiguration configuration)
    {
        var secret =
            configuration["Analytics:VisitorSalt"] ??
            configuration[AppSettings.Jwt.Key] ??
            "rccg-hope-house-site-visits";

        var address =
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var language =
            httpContext.Request.Headers.AcceptLanguage.ToString();

        var source =
            $"{visitDate:yyyy-MM-dd}|{address}|{userAgent}|{language}";

        var hash = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(secret),
            Encoding.UTF8.GetBytes(source));

        // 64 characters: SiteVisit.VisitorHashLength.
        return Convert.ToHexString(hash);
    }

    [GeneratedRegex(
        "bot|crawl|spider|slurp|headless|lighthouse|pagespeed|preview|" +
        "monitor|uptime|pingdom|facebookexternalhit|whatsapp|curl|wget|" +
        "python|java/|okhttp|postman|insomnia",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex RobotPattern();

    [GeneratedRegex(
        "Mobi|Android|iPhone|iPad|iPod",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex MobilePattern();
}

/// <summary>
/// Body sent by the website. Leave <c>Seconds</c> out to count a
/// page view; include it to add reading time.
/// </summary>
public record RecordSiteVisitRequest(int? Seconds);

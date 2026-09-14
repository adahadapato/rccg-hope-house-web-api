using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RccgHopeHouse.Application.Common.Behaviors;
using RccgHopeHouse.Application.Features.Auth.Commands;
using RccgHopeHouse.Application.Features.PastorPosts.Commands; // ← Import the command

namespace RccgHopeHouse.Application;

/// <summary>
/// Extension methods for registering Application-layer services.
/// This includes MediatR (CQRS pipeline) and FluentValidation (automatic request validation).
/// Keeps orchestration concerns close to where they're used.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers MediatR handlers and FluentValidation validators from the Application assembly.
    /// This enables the pipeline: Request → Validation → Handler → Response.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The modified IServiceCollection for chaining.</returns>
    /// <remarks>
    /// This method uses only DI abstractions, keeping Application decoupled from ASP.NET Core.
    /// Infrastructure or Program.cs should call this during composition.
    /// </remarks>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // ==================== MediatR (CQRS Pipeline) ====================
        /// <remarks>
        /// Auto-discovers all IRequestHandler, INotificationHandler implementations.
        /// Registers pipeline behaviors for validation, logging, caching (if added later).
        /// </remarks>
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            // cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>)); // Optional
        });

        // ==================== FluentValidation Pipeline ====================
        // This extension method is now available after installing FluentValidation.DependencyInjectionExtensions
        //services.AddValidatorsFromAssemblyContaining<DependencyInjection>(
        //    includeInternalTypes: false, // Only register public validators
        //    lifetime: ServiceLifetime.Singleton // Validators are stateless, safe as singletons
        //);

        // Register all FluentValidation validators
        // ⚠️ Must use a non-static type from this assembly for generic inference
        services.AddValidatorsFromAssemblyContaining<CreatePastorPostCommand>(
            ServiceLifetime.Transient, // Default: new instance per validation (fine for stateless validators)
            includeInternalTypes: false // Only register public validators
        );
        //services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();
        //services.AddValidatorsFromAssemblyContaining<UploadGalleryImageCommandValidator>();
        //services.AddValidatorsFromAssemblyContaining<DependencyInjection>();

        return services;
    }
}
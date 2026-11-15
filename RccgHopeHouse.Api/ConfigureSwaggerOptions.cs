using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
//using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RccgHopeHouse.Api;

/// <summary>
/// Configures Swagger generation with versioned documents and JWT security.
/// Uses IConfigureOptions to avoid BuildServiceProvider anti-pattern.
/// </summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        // Add a swagger document for each discovered API version
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = $"RCCG Hope House API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = description.IsDeprecated
                    ? "This API version is deprecated."
                    : "Clean Architecture API for church management.",

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


        }

        // Define JWT Bearer security scheme
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT access token: `Bearer eyJhbGciOiJIUzI1NiIs...`"
        });

        //options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        //{
        //     {
        //        new OpenApiSecurityScheme()
        //        {
        //            Reference = new OpenApiReference
        //            {
        //                Type = ReferenceType.SecurityScheme,
        //                Id = "Bearer"
        //            },
        //            Scheme = "oauth2",
        //            Name = "Bearer",
        //            In = ParameterLocation.Header,
        //        },
        //        new List<string>()
        //     }

        //});

        // Include XML comments from all project outputs
        var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml")
            .Where(f => f.Contains("RccgHopeHouse"));
        foreach (var xml in xmlFiles) options.IncludeXmlComments(xml);
    }
}
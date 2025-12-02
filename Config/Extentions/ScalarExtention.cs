using Asp.Versioning;
using Microsoft.OpenApi;

namespace UI_OpenAPI.Config.Extentions;
/// <summary>
/// کلاس افزودنی برای پیکربندی Scalar
/// </summary>
public static class ScalarExtention
{
    /// <summary>
    /// سرویس Scalar
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddScalarService(this IServiceCollection services)
    {
        services.AddOpenApi("v1");
        services.AddOpenApi("v2");
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((doc, context, cancellationToken) =>
            {
                doc.Info.Title = "مستندات API سایه بان";
                doc.Info.Version = "v1";
                doc.Info.Description = "API documentation for the application";

                // افزودن طرح امنیتی Bearer JWT برای Scalar
                doc.Components ??= new OpenApiComponents();
                doc.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                doc.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme. Enter your token below (without 'Bearer')."
                };

                // اعمال قفل سراسری به تمام عملیات‌ها
                doc.Security ??= new List<OpenApiSecurityRequirement>();
                return Task.CompletedTask;
            });
        });
        // تنظیم ورژن‌بندی API - سازگار با Asp.Versioning 8.1.0
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;

            // فقط از URL segment استفاده کن (بدون header یا query parameter)
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("x-api-version"),
                new QueryStringApiVersionReader("api-version")
            );
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        });
        return services;
    }
}
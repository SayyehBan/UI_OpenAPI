using Microsoft.OpenApi;

namespace UI_OpenAPI.Config.Extentions;
/// <summary>
/// // اتصال فایل XML به مستندات OpenAPI و افزودن احراز هویت Bearer برای Swagger UI
/// </summary>
public static class SwaggerExtention
{
    /// <summary>
    /// افزودن Swagger به خدمات
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSwaggerService(this IServiceCollection services)
    {
        var documentFilter = new DocumentFilter();
        var operationFilter = new OperationFilter();

        services.AddSwaggerGen(options =>
        {
            //// تعریف مستندات OpenAPI برای Swagger
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "مستندات ورژن 1 API سایه بان",
                Version = "v1",
                Description = "API documentation for the application with JWT authentication"
            });
            // تعریف مستندات OpenAPI برای Swagger
            options.SwaggerDoc("v2", new OpenApiInfo
            {
                Title = "مستندات ورژن 2 API سایه بان",
                Version = "v2",
                Description = "API documentation for the application with JWT authentication"
            });
            // افزودن طرح امنیتی Bearer JWT برای Swagger UI
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
            // اتصال فایل XML به مستندات OpenAPI
            var xmlFile = "OpenAPI.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (!File.Exists(xmlPath))
            {
                Console.WriteLine($"Warning: XML documentation file not found at: {xmlPath}");
            }
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            options.AddDocumentFilterInstance(documentFilter);
            options.AddOperationFilterInstance(operationFilter);
        });

        return services;
    }
}
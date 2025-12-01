using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Globalization;
using System.Text;
using UI_OpenAPI.Config;
using UI_OpenAPI.Config.Extentions;

var builder = WebApplication.CreateBuilder(args);

// تنظیم ApiBehaviorOptions
builder.Services.Configure<ApiBehaviorOptions>(apiBehaviorOptions =>
{
    apiBehaviorOptions.SuppressModelStateInvalidFilter = true;
});

// تنظیم RequestLocalizationOptions
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US") };
    options.RequestCultureProviders = new List<IRequestCultureProvider>();
});

// امکان آپلود فایل با حجم بالا
builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
    x.MultipartHeadersLengthLimit = int.MaxValue;
});

// گرفتن تنظیمات از appsettings.json
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);
var appSettings = appSettingsSection.Get<AppSettings>();

// افزودن احراز هویت JWT
builder.Services.AddAuthenticationJWT(appSettings!);

// دریافت اطلاعات از حافظه کش
builder.Services.AddMemoryCache();

// افزودن کنترلرها
builder.Services.AddControllers();

// افزودن سرویس‌های لازم برای مستندات OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScalarService();

// اتصال فایل XML به مستندات OpenAPI و افزودن احراز هویت Bearer برای Swagger UI
builder.Services.AddSwaggerService();
// تنظیم ورژن‌بندی API - سازگار با Asp.Versioning 8.1.0
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    // فقط از URL segment استفاده کن (بدون header یا query parameter)
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader()
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
// مپ کردن endpoint برای مستندات OpenAPI (برای Scalar)
app.UseSwagger();
app.MapOpenApi();

// افزودن رابط کاربری Scalar در مسیر /scalar
app.MapScalarApiReference("/scalar", options =>
{
    options.WithTitle("مستندات API سایه بان");
});

// فعال‌سازی Swagger و تنظیم Swagger UI
app.UseSwagger(c =>
{
    c.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
});
app.UseSwaggerUI(options =>
{
    // به‌صورت خودکار ورژن‌ها رو از IApiVersionDescriptionProvider بگیر
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            $"سایه بان - {description.GroupName.ToUpperInvariant()}");
  
    }
      options.DocumentTitle = "مستندات API سایه بان";
        options.DocExpansion(DocExpansion.None);
        options.RoutePrefix = "swagger"; // Swagger UI در مسیر /swagger
        options.DisplayRequestDuration();
        options.EnablePersistAuthorization();
        options.EnableFilter();
});
//}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
/// <summary>
/// فیلترمستندات
/// </summary>
public class DocumentFilter : IDocumentFilter
{
    static int count;
    /// <summary>
    /// فیلترمستندات
    /// </summary>
    public DocumentFilter() { Thread.Sleep(20); Console.WriteLine("DocumentFilter " + count++); }
    /// <summary>
    /// اعمال
    /// </summary>
    /// <param name="swaggerDoc"></param>
    /// <param name="context"></param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) { }
}
/// <summary>
/// فیلتر عملیات
/// </summary>
public class OperationFilter : IOperationFilter
{
    static int count;
    /// <summary>
    /// فیلتر عملیات
    /// </summary>
    public OperationFilter() { Thread.Sleep(20); Console.WriteLine("OperationFilter " + count++); }
    /// <summary>
    /// اعمال
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="context"></param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context) { }
}
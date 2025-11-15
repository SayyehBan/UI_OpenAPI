using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Microsoft.OpenApi;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerUI;
using UI_OpenAPI.Config;

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
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = appSettings?.JwtIssuer ?? throw new InvalidOperationException("JwtIssuer is not configured"),
        ValidAudience = appSettings?.JwtAudience ?? throw new InvalidOperationException("JwtAudience is not configured"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings?.JwtKey ?? throw new InvalidOperationException("JwtKey is not configured")))
    };
});

// دریافت اطلاعات از حافظه کش
builder.Services.AddMemoryCache();

// افزودن کنترلرها
builder.Services.AddControllers();

// افزودن سرویس‌های لازم برای مستندات OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
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

// اتصال فایل XML به مستندات OpenAPI و افزودن احراز هویت Bearer برای Swagger UI
builder.Services.AddSwaggerGen(options =>
{
    // تعریف مستندات OpenAPI برای Swagger
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "مستندات API سایه بان",
        Version = "v1",
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
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
// مپ کردن endpoint برای مستندات OpenAPI (برای Scalar)
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
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "مستندات API سایه بان");
    options.DocumentTitle = "مستندات API سایه بان";
    options.DocExpansion(DocExpansion.None);
    options.RoutePrefix = "swagger"; // Swagger UI در مسیر /swagger
    options.DisplayRequestDuration();
    options.EnablePersistAuthorization();
    options.EnableFilter();
});
//}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
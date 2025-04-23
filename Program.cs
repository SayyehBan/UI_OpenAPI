using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using UI_OpenAPI.Config;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;

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
        doc.Info.Title = "My API Documentation";
        doc.Info.Version = "v1";
        doc.Info.Description = "API documentation for the application";

        // افزودن طرح امنیتی Bearer JWT
        doc.Components ??= new OpenApiComponents();
        doc.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
        doc.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme."
        };

        // اعمال قفل سراسری به تمام عملیات‌ها
        doc.SecurityRequirements ??= new List<OpenApiSecurityRequirement>();
        doc.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new List<string>()
            }
        });

        return Task.CompletedTask;
    });
});

// اتصال فایل XML به مستندات OpenAPI
builder.Services.AddSwaggerGen(options =>
{
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
if (app.Environment.IsDevelopment())
{
    // مپ کردن endpoint برای مستندات OpenAPI
    app.MapOpenApi();

    // افزودن رابط کاربری Scalar در مسیر /scalar
    app.MapScalarApiReference("/scalar", options =>
    {
        options.WithTitle("My API Documentation")
               .WithDownloadButton(true)
               .WithTheme(ScalarTheme.Purple)
               .WithSidebar(true);
    });
}
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API Documentation");
    options.DocumentTitle = "My API Documentation";
    options.DocExpansion(DocExpansion.None);
    options.RoutePrefix = string.Empty; // Swagger UI در مسیر ریشه
    options.DisplayRequestDuration();
});


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
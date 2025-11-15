namespace UI_OpenAPI.Config;
/// <summary>
/// Represents application configuration settings related to authentication and other customizable options.
/// </summary>
/// <remarks>This class is typically used to bind configuration values from sources such as appsettings.json or
/// environment variables. It provides properties for JWT authentication parameters and additional settings that may be
/// required by the application.</remarks>
public class AppSettings
{
    /// <summary>
    /// اهراز اطلاعات
    /// </summary>
    public string? JwtIssuer { get; set; }
    /// <summary>
    /// آدرس
    /// </summary>
    public string? JwtAudience { get; set; }
    /// <summary>
    /// کلید
    /// </summary>
    public string? JwtKey { get; set; }
    /// <summary>
    /// تنظیمات
    /// </summary>
    public string? SomeOtherSetting { get; set; }
}

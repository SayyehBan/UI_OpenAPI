namespace UI_OpenAPI.Config;

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

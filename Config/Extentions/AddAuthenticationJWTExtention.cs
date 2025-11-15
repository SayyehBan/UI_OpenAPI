using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UI_OpenAPI.Config.Extentions;
/// <summary>
/// مراحل اهراز هویت برای لاگین
/// </summary>
public static class AddAuthenticationJWTExtention
{
    /// <summary>
    /// دسته بندی احرهز هویت
    /// </summary>
    /// <param name="services"></param>
    /// <param name="appSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddAuthenticationJWT(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddAuthentication(options =>
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
        return services;
    }

}
# 📚 پروژه آموزشی OpenAPI با ASP.NET Core 9

![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet)
![OpenAPI](https://img.shields.io/badge/OpenAPI-3.0-green)
![Swagger](https://img.shields.io/badge/Swagger-UI-brightgreen)
![Scalar](https://img.shields.io/badge/Scalar-UI-purple)
![License](https://img.shields.io/badge/license-MIT-blue)

این پروژه یک نمونه‌ی آموزشی برای آشنایی با **OpenAPI** در ASP.NET Core 9 است. هدف این پروژه نمایش نحوه‌ی استفاده از مستندات OpenAPI، ادغام توضیحات XML، و احراز هویت JWT با استفاده از دو رابط کاربری **Swagger UI** و **Scalar** است.

## ✨ ویژگی‌ها
- مستندات OpenAPI با توضیحات XML (مانند توضیحات فارسی برای endpointها و پارامترها)
- رابط کاربری **Swagger UI** و **Scalar** برای مشاهده و تست API به‌صورت تعاملی
- احراز هویت **JWT** با طرح امنیتی Bearer برای هر دو رابط کاربری
- پشتیبانی از آپلود فایل با حجم بالا
- تنظیمات ساده و قابل‌فهم برای یادگیری

## 🛠️ پیش‌نیازها
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- یک IDE مثل Visual Studio، VS Code یا JetBrains Rider
- (اختیاری) ابزاری برای تولید توکن JWT، مثل [jwt.io](https://jwt.io)

## 🚀 راه‌اندازی و اجرا
1. **کلون کردن پروژه**:
   ```bash
   git clone <repository-url>
   cd <repository-name>
   ```

2. **بازگرداندن بسته‌ها**:
   ```bash
   dotnet restore
   ```

3. **تنظیمات JWT**:
   فایل `appsettings.json` را باز کنید و تنظیمات JWT را وارد کنید:
   ```json
   {
     "AppSettings": {
       "JwtIssuer": "https://yourdomain.com",
       "JwtAudience": "https://yourapi.com",
       "JwtKey": "your-secure-key-with-at-least-32-characters"
     }
   }
   ```
   یا از فایل نمونه `appsettings.Development.json` استفاده کنید:
   ```json
   {
     "AppSettings": {
       "JwtIssuer": "https://example.com",
       "JwtAudience": "https://example.com",
       "JwtKey": "this-is-a-secure-key-for-testing-1234567890"
     }
   }
   ```

4. **اجرای پروژه**:
   ```bash
   dotnet run
   ```

5. **مشاهده مستندات**:
   - **Swagger UI**: به آدرس زیر بروید:
     ```
     http://localhost:5194/swagger
     ```
     در Swagger UI، می‌توانید توضیحات فارسی (مانند "تاریخ آب و هوا") را برای endpointها و پارامترها مشاهده کنید.
   - **Scalar**: به آدرس زیر بروید:
     ```
     http://localhost:5194/scalar
     ```
     در Scalar، مستندات OpenAPI نمایش داده می‌شوند، اما توضیحات XML ممکن است نمایش داده نشوند.

## 📸 تصاویر رابط کاربری

### Swagger UI
Swagger UI توضیحات XML (مانند توضیحات فارسی) را به‌خوبی نمایش می‌دهد و برای تست API با احراز هویت JWT مناسب است.

![Swagger UI - Image 1](images/1.jpg)
![Swagger UI - Image 2](images/2.jpg)

### Scalar
Scalar یک رابط کاربری مدرن برای نمایش مستندات OpenAPI است، اما ممکن است توضیحات XML را نمایش ندهد.

![Scalar - Image 3](images/3.jpg)
![Scalar - Image 4](images/4.jpg)

## 📖 نحوه‌ی تست API
1. **احراز هویت**:
   - در **Swagger UI**:
     - روی دکمه‌ی **Authorize** کلیک کنید.
     - توکن JWT خود را وارد کنید (فقط توکن، بدون کلمه‌ی `Bearer`).
   - در **Scalar**:
     - روی دکمه‌ی **Authorize** کلیک کنید و توکن JWT را وارد کنید.

2. **تست endpointها**:
   - endpoint `GET /api/WeatherForecast/GetWeatherForecast` را در هر دو رابط کاربری باز کنید.
   - پارامترهای موردنظر (مثل `Date` و `TemperatureC`) را وارد کنید.
   - درخواست را اجرا کنید تا پاسخ را ببینید.

## 🧩 ساختار پروژه
- **`Program.cs`**: تنظیمات اصلی پروژه، شامل مستندات OpenAPI، JWT، Swagger UI، و Scalar.
- **`Controllers/WeatherForecastController.cs`**: کنترلر نمونه برای پیش‌بینی آب و هوا با توضیحات XML.
- **`OpenAPI.xml`**: فایل مستندات XML که توضیحات فارسی را شامل می‌شود.
- **`appsettings.json`**: تنظیمات پروژه، از جمله JWT.
- **`images/`**: پوشه‌ی تصاویر رابط کاربری (Swagger و Scalar).

## 🔐 احراز هویت JWT
این پروژه از احراز هویت JWT با طرح امنیتی Bearer استفاده می‌کند. مراحل پیاده‌سازی در `Program.cs` به این صورت است:
1. **تنظیم احراز هویت**:
   ```csharp
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
           ValidIssuer = appSettings?.JwtIssuer,
           ValidAudience = appSettings?.JwtAudience,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings?.JwtKey))
       };
   });
   ```
2. **اعمال احراز هویت در مستندات OpenAPI**:
   ```csharp
   options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
   {
       Type = SecuritySchemeType.Http,
       Scheme = "bearer",
       BearerFormat = "JWT",
       Description = "JWT Authorization header using the Bearer scheme. Enter your token below (without 'Bearer')."
   });

   options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
   ```
3. **استفاده در کنترلرها**:
   - از Attribute `[Authorize]` برای اعمال احراز هویت در endpointها استفاده کنید:
     ```csharp
     [HttpGet("GetWeatherForecast")]
     [Authorize]
     public IActionResult GetWeatherForecast([FromQuery] WeatherForecast model)
     {
         // کد متد
     }
     ```

برای تست، می‌توانید توکن JWT را از ابزارهایی مثل [jwt.io](https://jwt.io) تولید کنید یا یک endpoint برای تولید توکن به پروژه اضافه کنید.

## 📝 نکات مهم
- **Swagger UI**: توضیحات XML (مانند "تاریخ آب و هوا") را نمایش می‌دهد و برای آموزش OpenAPI مناسب است.
- **Scalar**: ممکن است توضیحات XML را نمایش ندهد، اما رابط کاربری مدرن‌تری دارد.
- **پشتیبانی از فایل‌های بزرگ**: این پروژه امکان آپلود فایل با حجم بالا را فراهم می‌کند.
- **محیط توسعه**: مطمئن شوید که `ASPNETCORE_ENVIRONMENT` روی `Development` تنظیم شده است تا Swagger UI و Scalar فعال شوند.

## 🤝 مشارکت
اگر ایده‌ای برای بهبود این پروژه دارید، خوشحال می‌شوم که مشارکت کنید! لطفاً یک Pull Request ایجاد کنید یا Issue ثبت کنید.

## 📜 مجوز
این پروژه تحت [مجوز MIT](LICENSE) منتشر شده است.

## 🌟 تشکر
از شما برای استفاده از این پروژه‌ی آموزشی تشکر می‌کنم! اگر سوالی دارید، در بخش Issues مطرح کنید.

---

*ساخته شده با ❤️ توسط [سایه بان]*
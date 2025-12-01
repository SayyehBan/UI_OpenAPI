using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace UI_OpenAPI.Controllers.V2
{
    /// <summary>
    /// کنترلر پیش‌بینی آب و هوا
    /// </summary>
    [ApiVersion("2")]
    [ApiController]
    [Route("api/v{version:apiVersion=2}/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        /// <summary>
        /// سازنده کنترلر پیش‌بینی آب و هوا
        /// </summary>
        /// <param name="logger"></param>
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// متد برای دریافت پیش‌بینی آب و هوا
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<WeatherForecast> GetWeatherForecast([FromQuery] WeatherForecast model)
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        /// <summary>
        /// ثبت وضعیت
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostWeatherForecast([FromForm] WeatherForecast model)
        {
            if (model == null)
            {
                return BadRequest("Model cannot be null");
            }
            // Do something with the model
            return Ok(model);
        }
        /// <summary>
        /// Version 2
        /// </summary>
        /// <returns></returns>
        [HttpGet("version")]
        public ActionResult<string> ShowVersion()
        {
            return Ok("V2");
        }
    }
}

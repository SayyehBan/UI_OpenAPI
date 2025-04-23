namespace UI_OpenAPI
{
    /// <summary>
    ///model آب و هوا
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// تاریخ آب و هوا
        /// </summary>
        public DateOnly Date { get; set; }
        /// <summary>
        /// درجه حرارت به سانتی‌گراد
        /// </summary>
        public int TemperatureC { get; set; }
        /// <summary>
        /// درجه حرارت به فارنهایت
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        /// <summary>
        /// شرح وضعیت آب و هوا
        /// </summary>
        public string? Summary { get; set; }
    }
}

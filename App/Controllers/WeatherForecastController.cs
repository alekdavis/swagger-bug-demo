using Microsoft.AspNetCore.Mvc;

namespace App;

/// <summary>
/// Weather service.
/// </summary>
[ApiController]
[Route("[controller]")]
public class WeatherForecastController:ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    /// <summary>
    /// Weather service.
    /// </summary>
    public WeatherForecastController()
    {
    }

    /// <summary>
    /// Returns weather info for the city.
    /// </summary>
    /// <param name="city" example="Seattle">
    /// City.
    /// </param>
    /// <param name="scale" example="Celsius">
    /// Temperature type.
    /// </param>
    /// <returns>
    /// Weather info.
    /// </returns>
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get
    (
        [FromQuery] string city,
        [FromQuery] Scale scale
    )
    {
        return [.. Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),

            Temperature = new Temperature
            {
                Degrees = GetTemperature(scale),
                Scale = scale
            },
            
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })];
    }

    private int GetTemperature(Scale scale)
    {
        return scale switch
        {
            Scale.Celsius => Random.Shared.Next(-20, 35),
            Scale.Fahrenheit => Random.Shared.Next(-4, 105),
            _ => throw new ArgumentOutOfRangeException(nameof(scale), scale, null)
        };
    }
}

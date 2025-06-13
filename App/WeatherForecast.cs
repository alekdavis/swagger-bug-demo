using Common;

namespace App;

/// <summary>
/// Weather forecast info.
/// </summary>
public class WeatherForecast
{
    /// <summary>
    /// Date of the forecast in the specified city.
    /// </summary>
    public DateOnly? Date { get; set; }

    /// <summary>
    /// Forecasted temperature in the specified city.
    /// </summary>
    public virtual Temperature? Temperature { get; set; }

    /// <summary>
    /// Forecast summary.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// PROPERTY summary.
    /// </summary>
    public virtual Meta? Meta { get; set; }
}

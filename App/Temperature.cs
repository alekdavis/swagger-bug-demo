using Common;

namespace App;

/// <summary>
/// Holds temperature in Celsius and Fahrenheit.
/// </summary>
public class Temperature
{
    /// <summary>
    /// Temperature scale.
    /// </summary>
    public Scale Scale { get; set; } = Scale.Celsius;

    /// <summary>
    /// Temperature in C.
    /// </summary>
    public int Degrees { get; set; }
}

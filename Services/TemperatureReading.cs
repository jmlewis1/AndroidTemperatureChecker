namespace AndroidTemperatureChecker.Services;

public sealed class TemperatureReading
{
    public required string Name { get; init; }
    public required string Detail { get; init; }
    public double? Celsius { get; init; }

    public string Display => Celsius.HasValue
        ? $"{Celsius.Value:0.0} °C"
        : "—";
}

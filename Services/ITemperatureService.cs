namespace AndroidTemperatureChecker.Services;

public interface ITemperatureService
{
    Task<IReadOnlyList<TemperatureReading>> ReadAllAsync();
}

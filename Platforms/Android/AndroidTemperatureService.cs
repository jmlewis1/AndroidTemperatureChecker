using Android.Content;
using Android.OS;
using AndroidTemperatureChecker.Services;
using Application = Android.App.Application;

namespace AndroidTemperatureChecker.Platforms.Android;

public sealed class AndroidTemperatureService : ITemperatureService
{
    private const string ThermalRoot = "/sys/class/thermal";

    public Task<IReadOnlyList<TemperatureReading>> ReadAllAsync()
    {
        var results = new List<TemperatureReading>();

        var battery = TryReadBatteryTemperature();
        if (battery is not null)
            results.Add(battery);

        results.AddRange(ReadThermalZones());

        return Task.FromResult<IReadOnlyList<TemperatureReading>>(results);
    }

    private static TemperatureReading? TryReadBatteryTemperature()
    {
        try
        {
            var ctx = Application.Context;
            var filter = new IntentFilter(Intent.ActionBatteryChanged);
            using var intent = ctx.RegisterReceiver(null, filter);
            if (intent is null)
                return null;

            // BatteryManager.EXTRA_TEMPERATURE is in tenths of a degree Celsius.
            int tenths = intent.GetIntExtra(BatteryManager.ExtraTemperature, int.MinValue);
            if (tenths == int.MinValue)
                return null;

            return new TemperatureReading
            {
                Name = "Battery",
                Detail = "BatteryManager.EXTRA_TEMPERATURE",
                Celsius = tenths / 10.0,
            };
        }
        catch
        {
            return null;
        }
    }

    private static IEnumerable<TemperatureReading> ReadThermalZones()
    {
        var zoneDirs = EnumerateThermalZoneDirs();
        foreach (var dir in zoneDirs)
        {
            var reading = TryReadZone(dir);
            if (reading is not null)
                yield return reading;
        }
    }

    private static IReadOnlyList<string> EnumerateThermalZoneDirs()
    {
        try
        {
            if (!Directory.Exists(ThermalRoot))
                return Array.Empty<string>();

            return Directory.GetDirectories(ThermalRoot, "thermal_zone*")
                .OrderBy(NaturalZoneOrder)
                .ToList();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    private static TemperatureReading? TryReadZone(string dir)
    {
        try
        {
            var tempPath = Path.Combine(dir, "temp");
            var typePath = Path.Combine(dir, "type");

            if (!File.Exists(tempPath))
                return null;

            var rawTemp = File.ReadAllText(tempPath).Trim();
            if (!long.TryParse(rawTemp, out var milli))
                return null;

            var type = File.Exists(typePath)
                ? File.ReadAllText(typePath).Trim()
                : Path.GetFileName(dir);

            var zoneName = Path.GetFileName(dir);

            return new TemperatureReading
            {
                Name = string.IsNullOrWhiteSpace(type) ? zoneName : type,
                Detail = $"{zoneName} · {ThermalRoot}/{zoneName}/temp",
                Celsius = milli / 1000.0,
            };
        }
        catch
        {
            return null;
        }
    }

    private static int NaturalZoneOrder(string path)
    {
        var name = Path.GetFileName(path);
        var digits = new string(name.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out var n) ? n : int.MaxValue;
    }
}

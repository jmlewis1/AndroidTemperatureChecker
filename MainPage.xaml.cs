using System.Collections.ObjectModel;
using AndroidTemperatureChecker.Services;

namespace AndroidTemperatureChecker;

public partial class MainPage : ContentPage
{
    private readonly ITemperatureService _service;
    private readonly ObservableCollection<TemperatureReading> _readings = new();
    private IDispatcherTimer? _timer;

    public MainPage(ITemperatureService service)
    {
        InitializeComponent();
        _service = service;
        ReadingsList.ItemsSource = _readings;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = RefreshAsync();

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(2);
        _timer.Tick += async (_, _) => await RefreshAsync();
        _timer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timer?.Stop();
        _timer = null;
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await RefreshAsync();
        Refresher.IsRefreshing = false;
    }

    private async Task RefreshAsync()
    {
        var readings = await _service.ReadAllAsync();

        _readings.Clear();
        foreach (var r in readings)
            _readings.Add(r);

        LastUpdatedLabel.Text = $"Updated {DateTime.Now:HH:mm:ss}";
    }
}

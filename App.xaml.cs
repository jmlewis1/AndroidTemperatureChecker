namespace AndroidTemperatureChecker;

public partial class App : Application
{
    public App(MainPage mainPage)
    {
        InitializeComponent();
        MainPage = new NavigationPage(mainPage)
        {
            BarBackgroundColor = Color.FromArgb("#1E1E26"),
            BarTextColor = Colors.White,
        };
    }
}

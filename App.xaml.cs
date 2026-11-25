using EkseemMaarNet.Services;
using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet;

public partial class App : Application
{
    private readonly ContentService _content;
    private readonly MainViewModel _home;

    public App(ContentService content, MainViewModel home)
    {
        InitializeComponent();
        _content = content;
        _home = home;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell()) { Title = "Ekseem Maar Net" };

        // Look for newer content on launch and whenever the app comes back to the foreground (throttled),
        // and show today's affirmation if the day has changed while the app was in the background.
        window.Created += (_, _) => _ = _content.CheckForUpdatesAsync();
        window.Resumed += async (_, _) =>
        {
            _ = _content.CheckForUpdatesAsync();
            await _home.RefreshAffirmationAsync();
        };
        return window;
    }
}

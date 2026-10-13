using EkseemMaarNet.Services;

namespace EkseemMaarNet;

public partial class App : Application
{
    private readonly ContentService _content;

    public App(ContentService content)
    {
        InitializeComponent();
        _content = content;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell()) { Title = "Ekseem Maar Net" };

        // Look for newer content on launch and whenever the app comes back to the foreground (throttled).
        window.Created += (_, _) => _ = _content.CheckForUpdatesAsync();
        window.Resumed += (_, _) => _ = _content.CheckForUpdatesAsync();
        return window;
    }
}

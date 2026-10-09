namespace EkseemMaarNet.Services;

/// <summary>Opens web links in the in-app browser and tel: links in the phone dialer.</summary>
public static class Links
{
    public static async Task OpenAsync(string url)
    {
        try
        {
            if (url.StartsWith("tel:", StringComparison.OrdinalIgnoreCase))
                await Launcher.Default.OpenAsync(url);
            else
                await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception)
        {
            // No dialer (e.g. tablet/simulator) or no browser: nothing useful to do beyond not crashing.
        }
    }
}

using EkseemMaarNet.Models;

namespace EkseemMaarNet.Services;

/// <summary>Opens web links in the in-app browser and tel: links in the phone dialler.</summary>
public static class Links
{
    public static async Task OpenAsync(string url)
    {
        try
        {
            if (LinkRules.IsPhone(url))
            {
                if (PhoneDialer.Default.IsSupported)
                    PhoneDialer.Default.Open(url[4..]);
            }
            else if (LinkRules.IsWeb(url))
            {
                await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
            }
        }
        catch (Exception)
        {
            // No browser available: nothing useful to do beyond not crashing.
        }
    }
}

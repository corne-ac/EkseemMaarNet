using CommunityToolkit.Mvvm.ComponentModel;
using EkseemMaarNet.Services;

namespace EkseemMaarNet.ViewModels;

public partial class AboutViewModel(ContentService content) : PageViewModel
{
    [ObservableProperty]
    public partial string Disclaimer { get; set; } = "";

    [ObservableProperty]
    public partial string LastReviewed { get; set; } = "";

    public string Version => $"Version {AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";

    public async Task LoadAsync()
    {
        var library = await content.GetLibraryAsync();
        Disclaimer = library.Disclaimer;
        LastReviewed = library.LastReviewed;
    }
}

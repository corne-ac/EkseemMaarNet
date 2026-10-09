using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EkseemMaarNet.Services;

namespace EkseemMaarNet.ViewModels;

public partial class AboutViewModel(ContentService content) : ObservableObject
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

    [RelayCommand]
    private static Task OpenLink(string url) => Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);

    [RelayCommand]
    private static Task Back() => Shell.Current.GoToAsync("..");
}

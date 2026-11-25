using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EkseemMaarNet.Services;

namespace EkseemMaarNet.ViewModels;

/// <summary>Commands every inner page shares: the header's back button and tappable links.</summary>
public abstract partial class PageViewModel : ObservableObject
{
    [RelayCommand]
    private static Task Back() => Shell.Current.GoToAsync("..");

    [RelayCommand]
    private static Task OpenLink(string url) => Links.OpenAsync(url);
}

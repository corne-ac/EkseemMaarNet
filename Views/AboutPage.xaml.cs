using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet.Views;

public partial class AboutPage : ContentPage
{
    private readonly AboutViewModel _vm;

    public AboutPage(AboutViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}

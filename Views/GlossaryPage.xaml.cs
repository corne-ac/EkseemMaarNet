using EkseemMaarNet.Controls;
using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet.Views;

public partial class GlossaryPage : ContentPage
{
    private readonly GlossaryViewModel _vm;

    public GlossaryPage(GlossaryViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        EntriesList.BindSpan(Responsive.CardSpan);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}

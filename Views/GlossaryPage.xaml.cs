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
        EntriesList.SizeChanged += (_, _) =>
        {
            if (EntriesList.ItemsLayout is GridItemsLayout grid)
            {
                var span = Responsive.CardSpan(EntriesList.Width);
                if (grid.Span != span)
                    grid.Span = span;
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}

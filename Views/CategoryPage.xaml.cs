using EkseemMaarNet.Controls;
using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet.Views;

public partial class CategoryPage : ContentPage
{
    public CategoryPage(CategoryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        SectionsList.SizeChanged += (_, _) =>
        {
            if (SectionsList.ItemsLayout is GridItemsLayout grid)
            {
                var span = Responsive.CardSpan(SectionsList.Width);
                if (grid.Span != span)
                    grid.Span = span;
            }
        };
    }

#if DEBUG
    // Dev aid for screenshots: EMN_SCROLL=<section index> scrolls the page to that section on open.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (int.TryParse(Environment.GetEnvironmentVariable("EMN_SCROLL"), out var index) && index > 0)
        {
            await Task.Delay(900);
            if (BindingContext is CategoryViewModel vm && index < vm.Sections.Count)
                SectionsList.ScrollTo(index, position: ScrollToPosition.Start, animate: false);
        }
    }
#endif
}

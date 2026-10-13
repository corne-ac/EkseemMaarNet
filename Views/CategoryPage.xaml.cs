using EkseemMaarNet.Controls;
using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet.Views;

public partial class CategoryPage : ContentPage
{
    public CategoryPage(CategoryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        SectionsList.BindSpan(Responsive.CardSpan);
    }

#if DEBUG
    // Dev aid for screenshots: EMN_SCROLL scrolls the page to a section on open.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // "3" = section 3 at the top; "3c" = section 3 centred (leaves the previous card cut by the header).
        var scroll = Environment.GetEnvironmentVariable("EMN_SCROLL") ?? "";
        var centre = scroll.EndsWith('c');
        if (int.TryParse(scroll.TrimEnd('c'), out var index) && index > 0)
        {
            await Task.Delay(900);
            if (BindingContext is CategoryViewModel vm && index < vm.Sections.Count)
                SectionsList.ScrollTo(index, position: centre ? ScrollToPosition.Center : ScrollToPosition.Start, animate: false);
        }
    }
#endif
}

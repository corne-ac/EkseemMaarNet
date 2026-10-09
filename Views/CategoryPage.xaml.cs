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
}

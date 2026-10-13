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
#if DEBUG
        Debugging.DebugLaunch.ApplyToCategory(SectionsList, vm);
#endif
    }
}

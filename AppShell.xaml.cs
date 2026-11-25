using EkseemMaarNet.Views;

namespace EkseemMaarNet;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(CategoryPage), typeof(CategoryPage));
        Routing.RegisterRoute(nameof(GlossaryPage), typeof(GlossaryPage));
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
    }
}

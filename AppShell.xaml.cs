namespace EkseemMaarNet
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Register Routes
            Routing.RegisterRoute(nameof(MainFactorsPage), typeof(MainFactorsPage));
            Routing.RegisterRoute(nameof(GlossaryPage), typeof(GlossaryPage));
            Routing.RegisterRoute(nameof(WonderCuresPage), typeof(WonderCuresPage));
            Routing.RegisterRoute(nameof(TreatmentPage), typeof(TreatmentPage));
            Routing.RegisterRoute(nameof(RemediesPage), typeof(RemediesPage));
            Routing.RegisterRoute(nameof(SupplementsPage), typeof(SupplementsPage));
            Routing.RegisterRoute(nameof(CausesPage), typeof(CausesPage));
            Routing.RegisterRoute(nameof(BeveragesPage), typeof(BeveragesPage));
        
        }
    }
}

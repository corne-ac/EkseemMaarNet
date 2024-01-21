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
        }
    }
}

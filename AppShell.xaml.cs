namespace EkseemMaarNet
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Register Routes
            Routing.RegisterRoute(nameof(MainFactorsPage), typeof(MainFactorsPage));
        }
    }
}

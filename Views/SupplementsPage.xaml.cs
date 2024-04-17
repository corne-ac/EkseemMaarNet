using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet;

public partial class SupplementsPage : ContentPage
{
	public SupplementsPage(SupplementsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
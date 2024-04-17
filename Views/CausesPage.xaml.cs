using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet;

public partial class CausesPage : ContentPage
{
	public CausesPage(CausesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
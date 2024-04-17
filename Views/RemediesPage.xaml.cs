using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet;

public partial class RemediesPage : ContentPage
{
	public RemediesPage(RemediesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
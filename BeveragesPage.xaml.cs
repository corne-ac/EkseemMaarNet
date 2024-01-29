using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet;

public partial class BeveragesPage : ContentPage
{
	public BeveragesPage(BeveragesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
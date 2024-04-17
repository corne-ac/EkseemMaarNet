using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet;

public partial class MainFactorsPage : ContentPage
{
	public MainFactorsPage(MainFactorsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
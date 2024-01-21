using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet;

public partial class GlossaryPage : ContentPage
{
	public GlossaryPage(GlossaryViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
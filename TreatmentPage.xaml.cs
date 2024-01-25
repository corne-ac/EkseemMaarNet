using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet;

public partial class TreatmentPage : ContentPage
{
	public TreatmentPage(TreatmentViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
using CommunityToolkit.Mvvm.ComponentModel;
using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet;


public partial class WonderCuresPage : ContentPage
{
    public WonderCuresPage(WonderCuresViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
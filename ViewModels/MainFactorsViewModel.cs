

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EkseemMaarNet.ViewModels
{
    public partial class MainFactorsViewModel : ObservableObject
    {
        [RelayCommand]
        async Task home()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

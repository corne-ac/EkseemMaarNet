using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkseemMaarNet.ViewModels
{
    public partial class WonderCuresViewModel : ObservableObject
    {

        [RelayCommand]
        async Task home()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task openUrl(string url)
        {
            await Browser.OpenAsync(url); //Navigate back
        }
    }
}

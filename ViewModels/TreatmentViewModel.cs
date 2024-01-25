using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkseemMaarNet.ViewModels
{
    public partial class TreatmentViewModel : ObservableObject
    {
        [RelayCommand]
        async Task home()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

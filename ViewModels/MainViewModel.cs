﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkseemMaarNet.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {

        [RelayCommand]
        async Task mainFactors()
        {
            await Shell.Current.GoToAsync(nameof(MainFactorsPage));
        }

        [RelayCommand]
        async Task glossary()
        {
            await Shell.Current.GoToAsync(nameof(GlossaryPage));
        }

        [RelayCommand]
        async Task wonderCures()
        {
            await Shell.Current.GoToAsync(nameof(WonderCuresPage));
        }

        [RelayCommand]
        async Task treatment()
        {
            await Shell.Current.GoToAsync(nameof(TreatmentPage));
        }

    }
}

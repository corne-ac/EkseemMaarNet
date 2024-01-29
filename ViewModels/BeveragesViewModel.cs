using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkseemMaarNet.ViewModels
{
    public partial class BeveragesViewModel : ObservableObject
    {
        [ObservableProperty]
        string title = "Drinks and Beverages";
        
        [ObservableProperty]
        string paragraph = "";
        
        [ObservableProperty]
        Boolean rightVisible = true;
        [ObservableProperty]
        Boolean leftVisible = false;

        [ObservableProperty]
        string leftColumn = "<b>ADVANTAGEOUS</b><ul><li>Apple juice</li><li>Beet Juice</li><li>Celery Juice</li><li>Coconut Milk</li><li>Green Tea</li><li>Kombucha</li><li>Macha</li><li>Oolong Tea</li><li>Rooibos</li><li>Water (BO) Can improve symptoms immediately</li></ul><br/>";
        [ObservableProperty]
        string rightColumn = "<b>AVOID</b><ul><li>Black Tea</li><li>Coffee (AEH)</li><li>Energy Drinks</li><li>Grape Juice</li><li>Milk</li><li>Soda</li><br/></ul>";

        

        [RelayCommand]
        async Task home()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        void next()
        {
            Title = "Drinks | Alcohol (HHG)";
            Paragraph = "<p>Alcohol has a <b>dehydrating</b> effect and can be used in <b>moderation</b>.</p>" +
                        "<p> Alcohol has a <b>high-histamine</b> effect, as well as to be highly avoided for overall eczema health.</p>";
            LeftColumn = "<b>IN MODERATION</b><ul><li>Gin</li><li>Gin</li><li>Tequila</li><li>Brandy</li><li>Sake</li><li>Rum</li><br/></ul>";
            RightColumn = "<b>EXTREMELY <br>LIMITED</b><ul><li>Beer</li><li>Wine</li><li>Whiskey</li><li>Cocktails</li><br/></ul>";
            RightVisible = false;
            LeftVisible = true;
            
        }

        [RelayCommand]
        void previous()
        {
            Title = "Drinks and Beverages";
            Paragraph = "";
            LeftColumn = "<b>ADVANTAGEOUS</b><ul><li>Apple juice</li><li>Beet Juice</li><li>Celery Juice</li><li>Coconut Milk</li><li>Green Tea</li><li>Kombucha</li><li>Macha</li><li>Oolong Tea</li><li>Rooibos</li><li>Water (BO) Can improve symptoms immediately</li></ul><br/>";
            RightColumn = "<b>AVOID</b><ul><li>Black Tea</li><li>Coffee (AEH)</li><li>Energy Drinks</li><li>Grape Juice</li><li>Milk</li><li>Soda</li><br/></ul>";
            RightVisible = true;
            LeftVisible = false;

        }
    }
}

// < ! [CDATA[
//  < br />
// ]] >

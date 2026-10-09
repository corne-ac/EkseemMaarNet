using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EkseemMaarNet.Models;
using EkseemMaarNet.Services;

namespace EkseemMaarNet.ViewModels;

public partial class GlossaryViewModel(ContentService content) : ObservableObject
{
    private List<GlossaryEntry> _all = [];

    public ObservableCollection<GlossaryEntry> Entries { get; } = [];

    [ObservableProperty]
    public partial string Filter { get; set; } = "";

    public async Task LoadAsync()
    {
        if (_all.Count > 0)
            return;

        var library = await content.GetLibraryAsync();
        _all = library.Glossary;
        Apply();
    }

    partial void OnFilterChanged(string value) => Apply();

    private void Apply()
    {
        Entries.Clear();
        foreach (var entry in _all.Where(e => string.IsNullOrWhiteSpace(Filter)
                     || e.Term.Contains(Filter, StringComparison.OrdinalIgnoreCase)
                     || e.Meaning.Contains(Filter, StringComparison.OrdinalIgnoreCase)))
            Entries.Add(entry);
    }

    [RelayCommand]
    private static Task Back() => Shell.Current.GoToAsync("..");
}

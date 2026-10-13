using CommunityToolkit.Mvvm.ComponentModel;
using EkseemMaarNet.Models;
using EkseemMaarNet.Services;

namespace EkseemMaarNet.ViewModels;

public partial class GlossaryViewModel(ContentService content) : PageViewModel
{
    private List<GlossaryEntry> _all = [];

    /// <summary>Replaced as a whole on each keystroke, so the list redraws once.</summary>
    [ObservableProperty]
    public partial IReadOnlyList<GlossaryEntry> Entries { get; set; } = [];

    [ObservableProperty]
    public partial string Filter { get; set; } = "";

    public async Task LoadAsync()
    {
        if (_all.Count > 0)
            return;

        _all = (await content.GetLibraryAsync()).Glossary;
        Apply();
    }

    partial void OnFilterChanged(string value) => Apply();

    private void Apply() =>
        Entries = string.IsNullOrWhiteSpace(Filter)
            ? _all
            : _all.Where(e => e.Term.Contains(Filter, StringComparison.OrdinalIgnoreCase)
                              || e.Meaning.Contains(Filter, StringComparison.OrdinalIgnoreCase)).ToList();
}

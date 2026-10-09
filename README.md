# Ekseem Maar Net
A mobile app for people who live with the skin condition called "ekseem" in Afrikaans, also known as eczema (atopic dermatitis).

The app gives the family one easy place to check what helps and what to avoid — food, drinks, clothing, skin care, treatments and more. Someone doing the shopping can open the app, search for a product type (for example "coffee", "wool" or "vitamin D") and immediately see whether it is generally a good choice for eczema and why.

## Categories
| Tile | What it covers |
| --- | --- |
| Main Factors | The biggest drivers of flares |
| Wonder Cures | Biologics, JAK inhibitors and new non-steroid creams |
| Treatment | Guideline-based stepped care |
| Remedies | Home and natural remedies, with evidence verdicts |
| Supplements | What the evidence says about popular supplements |
| Causes | Triggers, allergens, environment, myths |
| Beverages | Drinks and alcohol |
| Food | Eating well, allergies and elimination diets |
| Water | Bathing, showering, hard water, swimming, wet wraps |
| Clothing | Fabrics, laundry and night-time scratching |
| Health | Sleep, exercise, infections and when to see a doctor |
| Motivation | The emotional side, family support and affirmations |

Plus a **Glossary** (abbreviations such as HHF, BO, AEH, HBUC and medical terms) and an **About** page with the disclaimer and rating legend.

Every item carries a rating — *Best option, Helpful, Usually fine, In moderation, Caution, Avoid* — with a short reason. Each category lists the sources it is based on.

## Sources
The original content came from personal research within the family. In October 2026 it was reviewed against current guidelines and evidence (AAD, NICE, EuroGuiDerm, Cochrane reviews and patient organisations such as the National Eczema Association and NHS). The sources for each page are listed at the bottom of that page in the app.

> **Not a medical app.** Ekseem Maar Net is a family passion project, made out of love for my brother. It shares general, surface-level information from public guidelines and research. It is not medical advice and does not give medical-aid or funding advice. Always talk to a doctor, dermatologist or pharmacist before starting, stopping or changing treatment.

## Tech
- .NET 10 / .NET MAUI 10 (Android and iOS), MVVM with CommunityToolkit.Mvvm
- Compiled bindings and XAML source generation
- All content lives in [`Resources/Raw/content.json`](Resources/Raw/content.json): edit that file to update information without touching any UI code
- One responsive `CategoryPage` template renders every category: one column on phones, two on tablets/landscape
- Icons: [Fluent UI System Icons](https://github.com/microsoft/fluentui-system-icons) (MIT)

### Project layout
```
Models/ContentModels.cs     content schema (Category, Section, Item, Rating) + JSON source-gen context
Services/ContentService.cs  loads content.json once; search across all categories
ViewModels/                 Main (tiles + search), Category (filters), Glossary, About
Views/                      MainPage, CategoryPage, GlossaryPage, AboutPage
Controls/                   PageHeader, RatingLegend, converters, responsive breakpoints
```

### Content format
```json
{
  "id": "food", "title": "Food", "subtitle": "…", "icon": "eee9", "intro": "…",
  "sections": [
    { "title": "Helpful", "intro": "", "items": [
      { "name": "Oily fish", "note": "…", "rating": "good", "tags": ["BO"] }
    ] }
  ],
  "sources": [ { "title": "…", "url": "https://…" } ]
}
```
`rating` is one of `best`, `good`, `ok`, `moderation`, `caution`, `avoid`, `info`. `icon` is a Fluent System Icons code point (hex). Items may include a `url` that shows a "Learn more" button.

### Build and run
```bash
dotnet build -t:Run -f net10.0-ios        # iOS simulator
dotnet build -t:Run -f net10.0-android    # Android emulator/device
```
The repo has its own `nuget.config` (nuget.org only), so it restores without any private feeds.

Debug-only helpers for UI checks (compiled out of Release builds):
- `EMN_START_ROUTE` opens a page on launch, e.g. `CategoryPage?id=food`
- `EMN_START_SEARCH` pre-fills the home search
- `EMN_SCROLL` scrolls a category page to a section index
- `EMN_SKIP_WELCOME=1` hides the first-launch welcome card

iOS: `SIMCTL_CHILD_EMN_START_ROUTE="CategoryPage?id=food" xcrun simctl launch booted com.corne.ekseemmaarnet`
Android: `adb shell setprop debug.mono.env "'EMN_START_ROUTE=CategoryPage?id=food'"` then launch the app.

## Origin
My brother has been suffering from eczema his entire life. This application makes it easier for the rest of the family to support him by giving an easy-to-understand source for what is and is not beneficial for his condition — especially for the things we eat, drink and use every day.

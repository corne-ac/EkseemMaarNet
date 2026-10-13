# Ekseem Maar Net
This is a mobile app for people that suffer from a skin condition called "ekseem" in Afrikaans, also known as "eczema".

The purpose of the app is to have an easy way to quickly check what helps eczema and what to stay away from: food, drinks, clothing, skin care, treatments and more. Someone doing the shopping can open the app, search for something like "coffee", "wool" or "vitamin D", and see straight away if it's generally okay for eczema and why.

> **Not a medical app.** Ekseem Maar Net is a family passion project, made for my brother. It shares general, surface-level information from public guidelines and research. It is not medical advice and doesn't give any medical-aid or funding advice. Always talk to a doctor, dermatologist or pharmacist before starting, stopping or changing treatment.

## Categories
| Tile | What's in it |
| --- | --- |
| Main Factors | The things that matter most |
| Wonder Cures | Newer treatments like biologics, JAK tablets and steroid-free creams |
| Treatment | What doctors recommend, step by step |
| Remedies | Home and natural remedies, and which ones are worth trying |
| Supplements | What the research says about vitamins and pills |
| Causes | Triggers, allergens, the environment, and some myths |
| Beverages | Drinks and alcohol |
| Food | Eating well, allergies and why not to cut foods out |
| Water | Baths, showers, swimming, bleach baths and wet wraps |
| Clothing | Fabrics, washing and night-time scratching |
| Health | Sleep, exercise, infections and when to see a doctor |
| Motivation | The emotional side, how family can help, and daily affirmations |

There's also a **Glossary** (our own tags like HHF, BO, AEH and HBUC, plus the medical words) and an **About** page.

Every item has a rating (*Best option, Helpful, Usually fine, In moderation, Caution, Avoid*) and a short reason. The bottom of each page lists where the information comes from.

## Sources
The information in the first version came from personal research with family members. In October 2026 everything was checked against current guidelines and research (AAD, NICE, the European and South African guidelines, Cochrane reviews, and groups like the National Eczema Association and the NHS), and anything outdated was fixed.

## Updating the content (no app release needed)
All the content lives in [`Resources/Raw/content.json`](Resources/Raw/content.json). The app ships with a copy of it, and also checks this file on the `main` branch for a newer version.

How it works:
1. The app opens straight away with the newest copy it has, either the bundled one or one it downloaded before.
2. On launch, and when it comes back to the foreground (at most once an hour), it fetches `content.json` from `main` on GitHub.
3. If that file has a higher `contentVersion` and is valid, the app saves it and the home screen refreshes. Pages opened after that show the new content.
4. If anything goes wrong (no internet, a bad file), the app quietly keeps what it has.

To publish a content change:
1. Edit `Resources/Raw/content.json` on a branch.
2. Bump `contentVersion`. We use the date plus a counter, e.g. `2026101301`, then `2026101302` for a second change on the same day.
3. Run `python3 scripts/validate_content.py` (CI runs it on every PR too, and fails if you forgot to bump the version).
4. Merge to `main`. Everyone gets it the next time they open the app.

Some safety rules are built in:
- `schemaVersion` protects older apps. Only raise it if the *shape* of the JSON changes in a way old app versions can't read. Then also raise `ContentLibrary.SupportedSchemaVersion` in the app and in `scripts/validate_content.py`, and ship an app update first.
- Downloaded content can only link to `https://` pages or `tel:` phone numbers. Anything else is stripped.
- New fields, categories or sections that need new UI still need an app release.

### Content format
```json
{
  "schemaVersion": 1,
  "contentVersion": 2026101301,
  "lastReviewed": "9 October 2026",
  "disclaimer": "…",
  "categories": [
    {
      "id": "food", "title": "Food", "tileTitle": "Food", "subtitle": "…", "icon": "eee9", "intro": "…",
      "sections": [
        { "title": "Good choices", "intro": "", "style": "", "items": [
          { "name": "Oily fish", "note": "…", "rating": "best", "tags": ["BO"], "url": null }
        ] }
      ],
      "sources": [ { "title": "…", "url": "https://…" } ]
    }
  ],
  "glossary": [ { "term": "BO", "meaning": "Best option", "detail": "…" } ]
}
```
- `rating`: `best`, `good`, `ok`, `moderation`, `caution`, `avoid` or `info`
- `icon`: a [Fluent System Icons](https://github.com/microsoft/fluentui-system-icons) code point in hex
- `style`: leave empty, `alert` for urgent sections (red outline), or `affirmations` for the list Home picks its daily affirmation from (exactly one section should have it)
- `url`: optional. Shows a "Learn more" button, or a "Call" button for `tel:` links

## GitHub Actions
| Workflow | When | What it does |
| --- | --- | --- |
| `pr-content.yml` | PRs that change `content.json` | Validates the content and checks `contentVersion` was bumped (takes seconds) |
| `pr-android.yml` | PRs to `main` (not content-only) | Builds the Android app |
| `pr-ios.yml` | PRs to `main` (not content-only) | Builds the iOS app for the simulator |
| `publish-android.yml` | Manual | Builds a signed `.aab` and can upload it to Google Play. **Not set up yet**, see the checklist at the top of the file |
| `publish-ios.yml` | Manual | Builds a signed `.ipa` and can upload it to TestFlight. **Not set up yet**, see the checklist at the top of the file |

The shared setup (Java or Xcode, .NET, NuGet cache, MAUI workload) lives in `.github/actions/setup-maui`.

## Tech
- .NET 10 / .NET MAUI 10 for Android and iOS, MVVM with CommunityToolkit.Mvvm
- Compiled bindings and XAML source generation
- One `CategoryPage` template shows every category: one column on phones, two on tablets
- Icons: [Fluent UI System Icons](https://github.com/microsoft/fluentui-system-icons) by Microsoft (MIT)

### Project layout
```
Models/ContentModels.cs     content format (Category, Section, Item, Rating) and JSON source generation
Services/ContentService.cs  loads content (bundled, cached or downloaded), checks for updates, search
Services/Links.cs           opens web links and phone numbers
ViewModels/                 Main (tiles, search, affirmation), Category (filters), Glossary, About
Views/                      MainPage, CategoryPage, GlossaryPage, AboutPage
Controls/                   PageHeader, FadeHost (scroll fade), WallpaperBackdrop, RatingTint, RatingLegend, breakpoints
Platforms/*/FadeHost.*.cs   native code for the transparent scroll fade
Debugging/DebugLaunch.cs    debug-only launch options for screenshots (below)
scripts/validate_content.py checks content.json before it reaches anyone
```

### Build and run
```bash
dotnet build -t:Run -f net10.0-ios        # iOS simulator
dotnet build -t:Run -f net10.0-android    # Android emulator or device
```
The repo has its own `nuget.config` (nuget.org only), so it restores without any private feeds.

Debug-only helpers for checking the UI (they're not in Release builds):
- `EMN_START_ROUTE` opens a page on launch, e.g. `CategoryPage?id=food`
- `EMN_START_SEARCH` fills in the home search
- `EMN_SCROLL` scrolls a category page to a section, e.g. `3`, or `3c` to centre it
- `EMN_SKIP_WELCOME=1` hides the welcome card
- `EMN_CONTENT_URL` checks a different content.json for updates, e.g. a branch's raw file, to test content before merging

iOS: `SIMCTL_CHILD_EMN_START_ROUTE="CategoryPage?id=food" xcrun simctl launch booted com.corne.ekseemmaarnet`
Android: `adb shell setprop debug.mono.env "'EMN_START_ROUTE=CategoryPage?id=food'"`, then open the app.

## Origin
My brother has been suffering from eczema for his entire life. This application is to make it easier for the rest of the family to support his condition by having an easy-to-understand place for what is and is not good for his eczema, especially when it comes to the things we eat, drink and use every day. We could be doing the shopping, open the app, and quickly check if something is okay for eczema or not.

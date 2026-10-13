#!/usr/bin/env python3
"""Validates Resources/Raw/content.json before it can reach users.

The app downloads content.json from main (see Services/ContentService.cs), so a broken file
on main would be a broken app for everyone. CI runs this on every PR.

Usage:
  python3 scripts/validate_content.py [path/to/content.json] [--base path/to/base/content.json]

--base: the same file from the target branch. If the content changed, contentVersion must go up.
"""
import json
import sys

SUPPORTED_SCHEMA = 1  # keep in step with ContentLibrary.SupportedSchemaVersion
RATINGS = {"info", "best", "good", "ok", "moderation", "caution", "avoid"}
SECTION_STYLES = {"", "alert"}


def fail(errors, message):
    errors.append(message)


def check_link(errors, where, url, allow_tel):
    ok = url.startswith("https://") or (allow_tel and url.startswith("tel:"))
    if not ok:
        fail(errors, f"{where}: link must be https:// {'or tel: ' if allow_tel else ''}({url})")


def validate(lib):
    errors = []
    if not isinstance(lib.get("schemaVersion"), int) or lib["schemaVersion"] > SUPPORTED_SCHEMA:
        fail(errors, f"schemaVersion must be an int <= {SUPPORTED_SCHEMA}")
    if not isinstance(lib.get("contentVersion"), int) or lib["contentVersion"] <= 0:
        fail(errors, "contentVersion must be a positive int (e.g. 2026101301)")
    for key in ("lastReviewed", "disclaimer"):
        if not str(lib.get(key, "")).strip():
            fail(errors, f"{key} is required")

    categories = lib.get("categories") or []
    if not categories:
        fail(errors, "at least one category is required")

    seen = set()
    for c in categories:
        cid = c.get("id", "")
        where = f"category '{cid or '?'}'"
        if not cid or not c.get("title"):
            fail(errors, f"{where}: id and title are required")
        if cid in seen:
            fail(errors, f"{where}: duplicate id")
        seen.add(cid)
        try:
            int(c.get("icon", ""), 16)
        except ValueError:
            fail(errors, f"{where}: icon must be a hex Fluent icon code point")
        for s in c.get("sections", []):
            swhere = f"{where} / section '{s.get('title', '?')}'"
            if not s.get("title"):
                fail(errors, f"{swhere}: title is required")
            if s.get("style", "") not in SECTION_STYLES:
                fail(errors, f"{swhere}: unknown style '{s.get('style')}'")
            if not s.get("items") and not s.get("intro"):
                fail(errors, f"{swhere}: needs items or an intro")
            for i in s.get("items", []):
                iwhere = f"{swhere} / item '{i.get('name', '?')}'"
                if not i.get("name"):
                    fail(errors, f"{iwhere}: name is required")
                if i.get("rating", "info") not in RATINGS:
                    fail(errors, f"{iwhere}: rating must be one of {sorted(RATINGS)}")
                if i.get("url"):
                    check_link(errors, iwhere, i["url"], allow_tel=True)
        for src in c.get("sources", []):
            check_link(errors, f"{where} / source '{src.get('title', '?')}'", src.get("url", ""), allow_tel=False)

    for g in lib.get("glossary", []):
        if not g.get("term") or not g.get("meaning"):
            fail(errors, f"glossary entry '{g.get('term', '?')}': term and meaning are required")
    return errors


def main(argv):
    args = [a for a in argv[1:] if not a.startswith("--")]
    path = args[0] if args else "Resources/Raw/content.json"
    base_path = argv[argv.index("--base") + 1] if "--base" in argv else None

    with open(path, encoding="utf-8") as f:
        lib = json.load(f)
    errors = validate(lib)

    if base_path:
        try:
            with open(base_path, encoding="utf-8") as f:
                base = json.load(f)
            changed = {k: v for k, v in lib.items() if k != "contentVersion"} != \
                      {k: v for k, v in base.items() if k != "contentVersion"}
            base_version = base.get("contentVersion", 0)
            if changed and lib.get("contentVersion", 0) <= base_version:
                fail(errors, f"content changed but contentVersion was not increased (base is {base_version})")
        except FileNotFoundError:
            pass  # no base file (e.g. first time the file exists on the target branch)

    if errors:
        print(f"content.json has {len(errors)} problem(s):")
        for e in errors:
            print(f"  - {e}")
        return 1

    items = sum(len(s.get("items", [])) for c in lib["categories"] for s in c.get("sections", []))
    print(f"content.json OK: v{lib['contentVersion']}, {len(lib['categories'])} categories, {items} items, "
          f"{len(lib.get('glossary', []))} glossary terms")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

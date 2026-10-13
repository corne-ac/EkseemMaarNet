#!/usr/bin/env python3
"""Validates Resources/Raw/content.json before it can reach users.

The app downloads content.json from main (see Services/ContentService.cs), so a broken file
on main would be a broken app for everyone. CI runs this on every PR.

Usage:
  python3 scripts/validate_content.py [path/to/content.json] [--base path/to/base/content.json]

--base: the same file from the target branch. If the content changed, contentVersion must go up.
"""
import argparse
import json
import sys

SUPPORTED_SCHEMA = 1  # keep in step with ContentLibrary.SupportedSchemaVersion
RATINGS = {"info", "best", "good", "ok", "moderation", "caution", "avoid"}
SECTION_STYLES = {"", "alert", "affirmations"}  # keep in step with ContentSection.Style


def check_link(errors, where, url, allow_tel):
    ok = url.startswith("https://") or (allow_tel and url.startswith("tel:"))
    if not ok:
        errors.append(f"{where}: link must be https:// {'or tel: ' if allow_tel else ''}({url})")


def validate(lib):
    errors = []
    if not isinstance(lib.get("schemaVersion"), int) or lib["schemaVersion"] > SUPPORTED_SCHEMA:
        errors.append(f"schemaVersion must be an int <= {SUPPORTED_SCHEMA}")
    if not isinstance(lib.get("contentVersion"), int) or lib["contentVersion"] <= 0:
        errors.append("contentVersion must be a positive int (e.g. 2026101301)")
    for key in ("lastReviewed", "disclaimer"):
        if not str(lib.get(key, "")).strip():
            errors.append(f"{key} is required")

    categories = lib.get("categories") or []
    if not categories:
        errors.append("at least one category is required")

    seen = set()
    for c in categories:
        cid = c.get("id", "")
        where = f"category '{cid or '?'}'"
        if not cid or not c.get("title"):
            errors.append(f"{where}: id and title are required")
        if cid in seen:
            errors.append(f"{where}: duplicate id")
        seen.add(cid)
        try:
            code_point = int(c.get("icon", ""), 16)
            if not (0 < code_point <= 0x10FFFF) or 0xD800 <= code_point <= 0xDFFF:
                raise ValueError
        except (TypeError, ValueError):
            errors.append(f"{where}: icon must be a hex Fluent icon code point (not a surrogate, max 10FFFF)")
        for s in c.get("sections", []):
            swhere = f"{where} / section '{s.get('title', '?')}'"
            if not s.get("title"):
                errors.append(f"{swhere}: title is required")
            if s.get("style", "") not in SECTION_STYLES:
                errors.append(f"{swhere}: unknown style '{s.get('style')}'")
            if not s.get("items") and not s.get("intro"):
                errors.append(f"{swhere}: needs items or an intro")
            for i in s.get("items", []):
                iwhere = f"{swhere} / item '{i.get('name', '?')}'"
                if not isinstance(i.get("name"), str) or not i["name"].strip():
                    errors.append(f"{iwhere}: name is required")
                if not isinstance(i.get("note", ""), str):
                    errors.append(f"{iwhere}: note must be text")
                tags = i.get("tags", [])
                if not isinstance(tags, list) or not all(isinstance(t, str) for t in tags):
                    errors.append(f"{iwhere}: tags must be a list of text")
                if i.get("url") is not None and not isinstance(i["url"], str):
                    errors.append(f"{iwhere}: url must be text")
                if i.get("rating", "info") not in RATINGS:
                    errors.append(f"{iwhere}: rating must be one of {sorted(RATINGS)}")
                if i.get("url"):
                    check_link(errors, iwhere, i["url"], allow_tel=True)
        for src in c.get("sources", []):
            check_link(errors, f"{where} / source '{src.get('title', '?')}'", src.get("url", ""), allow_tel=False)

    if not any(s.get("style") == "affirmations" and s.get("items")
               for c in categories for s in c.get("sections", [])):
        errors.append('one section needs "style": "affirmations" (Home picks its daily affirmation from it)')

    for g in lib.get("glossary", []):
        if not g.get("term") or not g.get("meaning"):
            errors.append(f"glossary entry '{g.get('term', '?')}': term and meaning are required")
    return errors


def main():
    parser = argparse.ArgumentParser(description="Validate content.json")
    parser.add_argument("path", nargs="?", default="Resources/Raw/content.json")
    parser.add_argument("--base", help="the same file from the target branch; if content changed, contentVersion must go up")
    args = parser.parse_args()

    with open(args.path, encoding="utf-8") as f:
        lib = json.load(f)
    errors = validate(lib)

    if args.base:
        try:
            with open(args.base, encoding="utf-8") as f:
                base = json.load(f)
            changed = {k: v for k, v in lib.items() if k != "contentVersion"} != \
                      {k: v for k, v in base.items() if k != "contentVersion"}
            base_version = base.get("contentVersion", 0)
            if changed and lib.get("contentVersion", 0) <= base_version:
                errors.append(f"content changed but contentVersion was not increased (base is {base_version})")
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
    sys.exit(main())

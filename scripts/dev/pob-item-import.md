# PoB item importer

Turns a Path of Building build code into `/item_give` commands for the local server
(`Agents/docs/dev/item-crafter.md`).

    node scripts/dev/pob-item-import.mjs            # reads artifacts/pob-build.xml
    node scripts/dev/pob-item-import.mjs <code>     # or a PoB code / XML path

It writes:

- `artifacts/pob-specs.json`: one item spec per rare or magic item (`Base`, `Rarity`, `ItemLevel`, `Quality`, `Mods`).
- `artifacts/pob-item-commands.txt`: the same specs as `/item_give {...}` lines to paste in game.

## What it resolves

- **The base**, from `BaseItemTypes` by name; a magic item's name carries its affix, so the longest base name that
  starts or ends the line wins.
- **Implicits**, from the base's own `Implicit_ModsKeys`. A special base such as Simplex Amulet stores its
  "modifiers cannot be changed / -2 prefixes" block as one multi-stat row, so those text lines are covered, not rematched.
- **Explicit modifiers**, from the item's text: a phrase rule names the stat, then the row is chosen among the base's
  `ModDomain` rows that carry exactly those stats, whose range contains the stated value, whose item classes admit the
  base, and whose generation type is a prefix (1) or suffix (2). A synthesised-generation row (3) is only a fallback.
- **Flags** (fractured, corrupted, mirrored, split, synthesised) and influences (elder, hunter, …) are reported, not
  written into the spec: the server sets those with `/item_state`.

## What it does not resolve

Each miss is printed with the reason rather than guessed:

- cluster-jewel lines (`Added Small Passive Skills grant: …`, `N Added Passive Skills are Jewel Sockets`,
  `Allocates …`), which need their own encoding;
- item properties PoB shows as mods (`Memory Strands`, `*BasePercentile`, catalyst lines);
- phrases with no rule yet (penetration, "Grants … per 2% Quality", trigger lines);
- values outside every row's range (`+127 to Strength`).

Adding a phrase is one entry in `RULES`; a wrong rule shows up as an unmatched line, never as a silent wrong stat.

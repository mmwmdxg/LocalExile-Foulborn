# Completion log: skills, currency and items (2026-11-05)

Working method requested: fill data-driven gaps server-side first, keep client data aligned where the project allows
it, and park everything that needs the real client for one later verification pass. Data comes from PoEDB pages read
through the desktop browser (web_fetch is blocked by DNS policy here).

## Done in this pass

| Area | Change | Where |
|---|---|---|
| Skills (gems) | Vaal Orb on a gem picks evenly among no change / +1 level over the cap (experience -1) / quality 23% / creatable Vaal variant, and corrupts it. | `Modules/Core/Game/Item/Items.cs` (`VaalGem`) |
| Currency | The Volatile Vaal Orb (`CurrencyHellscapeModValues`) sets a changeable unique's modifiers to the top of their range and corrupts it. | `Modules/Core/Game/Item/Items.cs` (`VolatileVaalMax`) |
| Currency | Tempering Orb (`CurrencyHeistWeaponEnchant`) replaces a weapon's enchantment with one `WeaponEnchantmentHeist*` row, keeping the rest. | `CurrencyEffects.Tempering` |
| Currency | Orb of Chance accepts normal and rare equipment, keeps a rare item's enchantment, and succeeds by default (`Game_CHANCE_SUCCESS_PERCENT`). | `UniqueCatalog.Chance` |
| Currency | Corrupting the Foulborn Rathpith Globe raises its two per-100-life modifiers from 3% to 4% (both `/item_state corrupted` and Vaal). | `ItemStates.Corrupt`, `ItemRolls.Vaal` |
| Items | Uniques whose mods carry `HasNoSockets` (Kaom's Heart, Kaom's Roots) are created without the Sockets component. | `UniqueCatalog.Create` |
| Items | Kaom's Heart rolls its maximum life in 780–1220 (the client's range) instead of a fixed 1000. | `UniqueItemSeed.cs` |
| Items | `/item_copy [mirrored]` duplicates the held item. | `CommandsFeature.cs` |

## Parked for the single live-verification pass

- Gem corruption outcomes and Vaal-variant switching in the real client.
- Volatile Vaal Orb on a real unique, including the corrupt tooltip and rerolled values.
- Orb of Chance on a rare body armour with a Tailoring enchant (enchant must survive).
- Kaom's Heart with no sockets at all (component removal) and its 780–1220 life.
- `/item_copy` copies: plain copy stays modifiable; the mirrored copy refuses currency.

## Next gaps (order kept: skills → currency → items)

1. Skills: remaining aura stats the server does not apply; conditional consecrated ground, herald/minion/banner work.
2. Currency: veiled modifiers (unveiling) are not modelled; "effects cannot be removed" backstop.
3. Items: other uniques' mechanics, Replica Headhunter and Enkindling.

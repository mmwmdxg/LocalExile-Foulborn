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
| Items | Replica Headhunter steals a magic monster's affixes at its 20% per-kill chance (Headhunter's rare path unchanged). | `UniqueBeltsFeature.StealMonsterMods`, `StolenMods` |
| Items | Aegis Aurora: 2% of armour rating recovered as energy shield when blocking. | `SpiritChieftains.Rules.BlockRecovery` |
| Items | Doryani's Prototype: armour applies to lightning damage taken, your lightning resistance no longer applies to it, and nearby enemies use your lightning resistance. | `PlayerHitMitigation.ArmourPercent`, `Player.Resistance`, `MonsterMods` |
| Items | The Brass Dome: critical strikes against the character deal no extra damage. | `MonsterMods.SelfCriticalReduction` |
| Skills | `always_ignite` now forces an ignite on any fire-damage hit (the chance roll is skipped). | `MonsterIgnites.IgniteFromHit` |
| Items | `chaos_damage_does_not_bypass_energy_shield` (Shavronne's Wrappings and kin) lets the shield absorb chaos hits. | `Antiquarian.DamagePlayerHit` |
| Skills | `always_shock` and `always_freeze` skip the chance roll on a qualifying hit (the Inflict path was already wired). | `MonsterElementalAilments.Inflict` |
| Skills | `base_cannot_be_frozen/chilled/ignited/shocked` now count as ailment immunity on the player (the plain `cannot_be_*` forms already did). | `PlayerElementalAilments.Immune` |
| Skills | `movement_speed_cannot_be_reduced_below_base` keeps the character at base speed under slows. | `ClientState.ApplyMovementSpeedAt` |
| Skills | `action_speed_cannot_be_reduced_below_base` does the same for action speed (Garb of the Ephemeral, Kaom's Roots). | `PlayerAilments.PlayerActionFactor` |
| Skills | `cannot_gain_endurance/frenzy/power_charges` refuse positive charge gains (losses still apply). | `ChargeSession.Apply` |
| Skills | A cursed state (`/curse [seconds]`, module-side per connection) feeds `while_cursed` / `while_not_cursed` block, spell block and movement speed modifiers. | `Passive/CurseState.cs`, `Progression.PassiveBonus`, `CommandsFeature` |
| Skills | Consecrated ground now feeds `additional_block_%_while_on_consecrated_ground` through `FlaskGrounds.PlayerOnConsecrated`. | `Progression.PassiveBonus` |
| Items | Gorgon's Gaze: `additional_physical_damage_reduction_%_while_moving` lowers physical hits while the player walks. | `Monsters.MonsterSwing` |
| Items | Malachai's Loop: `add_power_charge_on_skill_hit_%` was missing from the charge rules and now rolls on a hit. | `ChargeGains` |
| Items | Last Resort: `accuracy_rating_+%_when_on_low_life` joins the accuracy pool while life is at most 35% (the low-life flag is passed into PassiveBonus). | `Progression.PassiveBonus` |
| Items | Ahn's Might: `accuracy_rating_while_at_maximum_frenzy_charges` applies while the frenzy charges are full. | `Progression.PassiveBonus` |
| Items | `gain_iron_reflexes_while_at_maximum_frenzy_charges` grants Iron Reflexes while frenzy charges are full. | `Progression.PassiveBonus` |
| Items | `cannot_be_ignited/shocked_while_at_maximum_endurance_charges` count as immunity while endurance charges are full (a get-function wrapper, no new signatures). | `AilmentImmunity.WithEnduranceAilmentImmunity` |
| Skills | Onslaught: `gain_onslaught_while_at_maximum_endurance_charges` gives +20% action speed while the charges are full, and the player tick publishes/withdraws the icon (BuffDefinitions 215). | `Resources/Onslaught.cs`, `PlayerAilments.PlayerActionFactor`, `Player.RunPlayerTick` |
| Skills | `gain_onslaught_on_hit_chance_while_at_maximum_frenzy_charges_%` rolls Onslaught (4 s) on a hit while frenzy charges are full. | `Charges.ChargesOnHit` |
| Skills | Mind over Matter: `damage_taken_goes_to_mana_%` (keystone, or the at-maximum-power-charges grant) pays part of the life damage from mana; short mana falls back to life. | `Antiquarian.DamagePlayerHit` |
| Skills | Augury of Penitence (Inquisitor and Scion) now applies its increased elemental damage taken as a passive enemy aura, using the existing consumer. | `PassiveBuffAuras.EnemyAuraTemplates` |
| Currency | Tainted Chaos and Tainted Exalted Orbs reroll/add affixes on an already corrupted rare item (the ordinary orbs refuse corrupted targets). | `CurrencyEffects.TaintedChaos/TaintedExalted`, `ItemRolls.TryModifiable` |
| Currency | Tainted Armourer's Scrap and Tainted Whetstone add quality to already corrupted armour/weapons; the ordinary ones now refuse corrupted items. | `CurrencyEffects.TaintedScrap/TaintedWhetstoneItem` |
| Currency | Tainted Chromatic, Fusing and Jeweller's Orbs reroll colours/links/socket count on corrupted items; ordinary socket currencies now refuse them. | `CurrencyEffects.TaintedChromatic/TaintedFusing/TaintedJewellers` |
| Currency | Tainted Mythic Orb upgrades an already corrupted normal/rare item to a same-base unique. | `CurrencyEffects.TaintedMythic`, `UniqueCatalog.Chance` |
| Skills | Occultist nearby-enemy auras now reduce cold and chaos resistance, fed through a new monster field into the hit resistance | `EnemyDebuffs.Rules`, `MonsterMods`, `ClientState` |
| Skills | Occultist's nearby chilled enemies deal 10% less hit damage (applied only while the monster is chilled). | `Monsters.MonsterSwing`, `MonsterElementalAilments.Chilled` |

## Parked for the single live-verification pass

- Gem corruption outcomes and Vaal-variant switching in the real client.
- Volatile Vaal Orb on a real unique, including the corrupt tooltip and rerolled values.
- Orb of Chance on a rare body armour with a Tailoring enchant (enchant must survive).
- Kaom's Heart with no sockets at all (component removal) and its 780–1220 life.
- `/item_copy` copies: plain copy stays modifiable; the mirrored copy refuses currency.

## Data gaps found while looking for the next batch

- Veiled modifiers: the local Mods.dat has the 14 placeholder rows (domain 26, stats `veiled_mod_type`/`veiled_mod_seed`)
  but no link from a veil type to the concrete modifier rows, so an accurate unveiling pool cannot be built from local
  data alone. PoEDB's Veiled modifier page lists the pools as text; mapping it onto Mods Ids is a separate data job.
- Regrading Lens: `AlternateQualityTypes.json` only carries jewellery and map quality rows, not the gem quality types
  (Anomalous/Divergent/Phantasmal), so a quality-type reroll has no target table.
- Enkindling/Instilling: already implemented in `FlaskEnchantments.Apply` (GenerationType 21/22 pools); the status line
  that listed Enkindling as missing was stale and is corrected.

## Next gaps (order kept: skills → currency → items)

1. Skills: remaining aura stats the server does not apply; conditional consecrated ground, herald/minion/banner work.
2. Currency: veiled modifiers (unveiling) are not modelled; "effects cannot be removed" backstop.
3. Items: other uniques' mechanics, Replica Headhunter and Enkindling.

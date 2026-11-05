# Live verification checklist (2026-11-05 pass)

Everything below was implemented without a client run. Restart `scripts/dev/live.ps1` first, then work down the
list. Each row: what to do, what should happen, and what to send me if it does not.

## 0. Before starting

- Restart the stack (`scripts/dev/live.ps1`) so the Gateway, modules and data reload.
- Confirm the client is 3.29.3b and you are in a town or hideout with inventory space.

## 1. Currency

| # | Do | Expect |
|---|---|---|
| 1.1 | `/gem Fireball 20 20`, pick it up, use a Vaal Orb | One of: unchanged, level 21, 23% quality, or Vaal variant; item shows corrupted |
| 1.2 | `/item Volatile Vaal Orb`, use on an uncorrupted unique | Every modifier at its range top, item corrupted |
| 1.3 | `/item Glorious Plate`, pick up, use a Tailoring Orb, then an Orb of Chance | Becomes Kaom's Heart and keeps the enchant |
| 1.4 | `/item Tainted Chaos Orb` on a corrupted rare | Affixes rerolled |
| 1.5 | Same orb on an uncorrupted rare | Refused, orb not spent |
| 1.6 | `/item Tainted Exalted Orb` on corrupted rare | One affix added |
| 1.7 | `/item Tainted Armourer's Scrap` on corrupted armour | +1% quality |
| 1.8 | `/item Tainted Mythic Orb` on corrupted rare | Becomes a same-base unique |
| 1.9 | Normal `/item Orb of Chance` on a corrupted item | Refused |

## 2. Items

| # | Do | Expect |
|---|---|---|
| 2.1 | `/item Kaom's Heart` | No socket holes at all; life within 780–1220 |
| 2.2 | `/item_copy` with an item held | Plain copy, still modifiable |
| 2.3 | `/item_copy mirrored` | Copy marked corrupted/mirrored and refuses currency |
| 2.4 | `/item Aegis Aurora`, block a hit | Energy shield recovers 2% of armour |
| 2.5 | `/item Doryani's Prototype`, take a lightning hit | Armour mitigates it; your lightning resistance does not |
| 2.6 | Same, kill a nearby monster | Its lightning resistance reads as yours |
| 2.7 | `/item The Brass Dome`, get crit | No extra critical damage |
| 2.8 | `/item Replica Headhunter`, kill magic monsters | Affixes stolen on a 20% roll |
| 2.9 | `/item Gorgon's Gaze`, walk while hit physically | Reduced damage only while walking |
| 2.10 | `/item Last Resort`, drop below 35% life | Accuracy rises |
| 2.11 | `/item Ahn's Might`, hold max frenzy charges | Accuracy rises |

## 3. Skills and states

| # | Do | Expect |
|---|---|---|
| 3.1 | `/curse 30 vulnerability` with Atziri's Mirror | Spell block rises for 30 s; 20% more damage taken |
| 3.1b | `/curse 30 temporal_chains` | Action speed drops 20% |
| 3.2 | `/curse` | State cleared, block returns |
| 3.3 | Stand on consecrated ground with the block stat | Attack block rises |
| 3.4 | Hold max endurance charges with the immunity stat | Ignite and shock cannot be applied |
| 3.5 | `always_ignite` item/skill, hit with fire | Ignite lands without a chance roll |
| 3.6 | `always_shock` / `always_freeze` with the matching damage | Ailment lands without a chance roll |
| 3.7 | `base_cannot_be_*` item, get hit by that ailment | No ailment applied |
| 3.8 | Movement/action speed floor item under chill | Speed stays at base |
| 3.9 | Chaos damage with `chaos_damage_does_not_bypass_energy_shield` | Energy shield absorbs it |
| 3.10 | Hold max endurance charges with the Onslaught grant | Onslaught icon appears, action speed +20% |
| 3.11 | Max frenzy charges + `gain_onslaught_on_hit_chance_...`, hit | Onslaught icon appears on the roll for 4 s |
| 3.12 | Mind over Matter keystone, take a hit | Part of it comes off mana, not life |
| 3.13 | Mind over Matter only while at max power charges | Mana share only with full power charges |
| 3.14 | Vaal Pact keystone, leech life | Life refills instantly instead of over time |
| 3.15 | Vaal Pact only while at max endurance charges | Instant leech only with full endurance charges |
| 3.16 | Rampage grant with max endurance charges | Rampage counter stays up while the charges are full |
| 3.17 | Get hit by a monster with a curse-on-hit affix | Cursed state applies for 5 s (while_cursed modifiers change) |
| 3.18 | Vulnerability curse on hit | 20% more damage taken while it lasts |
| 3.19 | Temporal Chains curse on hit | Action speed drops 20% while it lasts |
| 3.20 | Two curse sources, one additional curse allowed | Both curses stay; the third replaces the oldest |
| 3.21 | Flammability/Frostbite/Conductivity curse on hit | The matching resistance is 20 lower while it lasts |
| 3.22 | `/curse 30 enfeeble`, hit a monster | Damage dealt is about 10% lower |
| 3.23 | `/curse 30 flammability` | Curse icon appears on the player and clears when it lapses |
| 3.24 | `/monster_curse vulnerability 10` then hit a nearby monster | It takes about 20% more damage for 10 s |
| 3.25 | `/monster_curse vulnerability` next to a hexproof monster | It refuses (no curse) |
| 3.26 | Cast a Vulnerability curse gem near monsters | Nearby monsters take about 20% more damage for 8 s |
| 3.27 | Cast Assassin's Mark near monsters | Marked monsters take extra damage |

## Reporting

For each failure send: the row number, what you did, what happened, and the last 20 lines of
`logs/live-gateway.log`. Failures will be fixed in the order reported, with the checklist row updated.

import fs from 'node:fs/promises';
import path from 'node:path';

const TABLES = path.join('data', 'tables');
const read = async (n) => JSON.parse(await fs.readFile(path.join(TABLES, n), 'utf8'));

export async function loadTables() {
  const [stats, mods, bases, classes] = await Promise.all(
    ['Stats.json', 'Mods.json', 'BaseItemTypes.json', 'ItemClasses.json'].map(read));
  return { stats, mods, bases, classes, statId: stats.map(s => s.Id) };
}

const N = (s) => Number(String(s).replace(/[^0-9.-]/g, ''));
const STR = { Strength: 'strength', Dexterity: 'dexterity', Intelligence: 'intelligence' };

// One PoB mod line: the stats its text names (in row order) and the values it states.
const RULES = [
  [/^\+(\d+) to (Strength|Dexterity|Intelligence)$/, (m) => [[ 'additional_' + STR[m[2]] ], [N(m[1])]]],
  [/^\+(\d+) to all Attributes$/i, (m) => [['additional_all_attributes'], [N(m[1])]]],
  [/^\+(\d+) to (Strength|Dexterity|Intelligence) and (Strength|Dexterity|Intelligence)$/,
    (m) => [['additional_' + STR[m[2]] + '_and_' + STR[m[3]]], [N(m[1])]]],
  [/^(\d+)% increased maximum Life$/, (m) => [['maximum_life_+%'], [N(m[1])]]],
  [/^(\d+)% increased maximum Energy Shield$/, (m) => [['maximum_energy_shield_+%'], [N(m[1])]]],
  [/^(\d+)% increased Energy Shield$/, (m) => [['local_energy_shield_+%'], [N(m[1])]]],
  [/^(\d+)% increased Strength$/, (m) => [['strength_+%'], [N(m[1])]]],
  [/^(\d+)% increased Attributes$/, (m) => [['all_attributes_+%'], [N(m[1])]]],
  [/^\+(\d+) to maximum Life$/, (m) => [['base_maximum_life'], [N(m[1])]]],
  [/^\+(\d+) to maximum Energy Shield$/, (m) => [['base_maximum_energy_shield'], [N(m[1])]]],
  [/^\+(\d+)% to maximum Fire Resistance$/, (m) => [['base_maximum_fire_damage_resistance_%'], [N(m[1])]]],
  [/^([+-]?\d+)% to Chaos Resistance$/, (m) => [['base_chaos_damage_resistance_%'], [N(m[1])]]],
  [/^([+-]?\d+)% to Lightning Resistance$/, (m) => [['base_lightning_damage_resistance_%'], [N(m[1])]]],
  [/^([+-]?\d+)% to Fire Resistance$/, (m) => [['base_fire_damage_resistance_%'], [N(m[1])]]],
  [/^\+(\d+)% to Fire and Chaos Resistances$/, (m) => [['fire_and_chaos_damage_resistance_%'], [N(m[1])]]],
  [/^(\d+)% increased Movement Speed$/, (m) => [['base_movement_velocity_+%'], [N(m[1])]]],
  [/^(\d+)% increased Movement Speed if you haven.t been Hit Recently$/, (m) => [['movement_velocity_while_not_hit_+%'], [N(m[1])]]],
  [/^(\d+)% increased Global Accuracy Rating$/, (m) => [['accuracy_rating_+%'], [N(m[1])]]],
  [/^\+(\d+) to Accuracy Rating$/, (m) => [['accuracy_rating'], [N(m[1])]]],
  [/^(\d+)% increased Elemental Damage with Attack Skills$/, (m) => [['elemental_damage_with_attack_skills_+%'], [N(m[1])]]],
  [/^\+(\d+)% to Damage over Time Multiplier$/, (m) => [['dot_multiplier_+'], [N(m[1])]]],
  [/^(\d+)% increased Damage over Time$/, (m) => [['damage_over_time_+%'], [N(m[1])]]],
  [/^(\d+)% increased Fire Damage$/, (m) => [['fire_damage_+%'], [N(m[1])]]],
  [/^(\d+)% increased Physical Damage$/, (m) => [['local_physical_damage_+%'], [N(m[1])]]],
  [/^(\d+)% chance to Ignite$/, (m) => [['base_chance_to_ignite_%'], [N(m[1])]]],
  [/^(\d+)% increased Area of Effect per 50 Strength$/, (m) => [['area_of_effect_+%_per_50_strength'], [N(m[1])]]],
  [/^(\d+)% increased Damage per 15 Strength$/, (m) => [['damage_+%_per_15_strength'], [N(m[1])]]],
  [/^Quality does not increase Physical Damage$/, () => [['local_quality_does_not_increase_physical_damage'], [1]]],
  [/^Item sells for much more to vendors$/, () => [['local_item_sell_price_doubled'], [1]]],
  [/^Implicit Modifiers Cannot Be Changed$/, () => [['local_implicit_mod_cannot_be_changed'], [1]]],
  [/^\+?(-?\d+) Prefix Modifiers? allowed$/, (m) => [['local_maximum_prefixes_allowed_+'], [N(m[1])]]],
  [/^\+?(-?\d+) Suffix Modifiers? allowed$/, (m) => [['local_maximum_suffixes_allowed_+'], [N(m[1])]]],
  [/^Has (\d+) Abyssal Socket$/, (m) => [['local_has_X_abyss_sockets'], [N(m[1])]]],
];

export function mapLine(text) {
  for (const [re, build] of RULES) {
    const m = text.match(re);
    if (m) { const [ids, values] = build(m); return { ids, values }; }
  }
  return null;
}

const decodeEntities = (s) => s.replace(/&apos;/g, "'").replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&quot;/g, '"');
const FLAGS = {
  'Fractured Item': 'fractured', 'Corrupted': 'corrupted', 'Mirrored': 'mirrored', 'Split': 'split',
  'Synthesised Item': 'synthesised', 'Unidentified': 'unidentified',
};
const PROP = new Set(['Quality', 'Item Level', 'Sockets', 'Implicits', 'Limited to', 'Stack Size', 'Level', 'Armour',
  'Evasion', 'Energy Shield', 'Ward', 'Physical Damage', 'Elemental Damage', 'Critical Strike Chance', 'Attacks per Second',
  'Weapon Range', 'Intangibility', 'Unique ID', 'Has Alt Variant', 'Catalyst', 'CatalystQuality', 'Memory Strands',
  'EnergyShieldBasePercentile', 'ArmourBasePercentile', 'Cluster Jewel Skill', 'Cluster Jewel Node Count', 'LevelReq']);

export function parseItems(xml) {
  const items = [];
  for (const m of xml.matchAll(/<Item id="(\d+)">([\s\S]*?)<\/Item>/g)) {
    const lines = decodeEntities(m[2]).replace(/\r/g, '').split('\n').map((s) => s.trim()).filter(Boolean);
    const start = lines.findIndex((s) => /^Rarity:/.test(s));
    if (start < 0) continue;
    items.push({ id: m[1], rarity: lines[start].split(':')[1].trim(), lines, start });
  }
  return items;
}

function findBase(lines, start, baseByName, bases) {
  for (let i = start + 1; i < Math.min(lines.length, start + 6); i++) {
    const hit = baseByName.get(lines[i]);
    if (hit) return { base: hit, name: lines[start + 1], index: i };
  }
  // A magic item's name carries its affix ("Infusing Baselard", "Bronze Tower Shield of the Bear"): the longest
  // base name that prefixes the line is the base, and the line before it is the item's name.
  for (let i = start + 1; i < Math.min(lines.length, start + 6); i++) {
    let best = null;
    for (const b of bases) {
      const line = lines[i].toLowerCase(), name = b.Name.toLowerCase();
      // A magic name is the affix plus the base, and the affix may sit on either side ("Infusing Baselard",
      // "Bronze Tower Shield of the Bear").
      if ((line.startsWith(name) || line.endsWith(name)) && (!best || b.Name.length > best.Name.length)) best = b;
    }
    if (best) return { base: best, name: lines[start + 1], index: i };
  }
  return null;
}

export async function run({ xmlFile = path.join('artifacts', 'pob-build.xml'), outFile = path.join('artifacts', 'pob-specs.json') } = {}) {
  const t = await loadTables();
  const xml = await fs.readFile(xmlFile, 'utf8');
  const baseByName = new Map(t.bases.map((b, i) => [b.Name, { row: i, ...b }]));
  const classIds = t.classes.map((c) => c.Id);
  const byStat = new Map();
  for (const row of t.mods) {
    for (let i = 1; i <= 8; i++) {
      const key = row['StatsKey' + i];
      if (key === null || key === undefined) continue;
      const id = t.statId[key];
      if (!byStat.has(id)) byStat.set(id, []);
      byStat.get(id).push(row);
    }
  }
  const rowStats = (row) => {
    const out = [];
    for (let i = 1; i <= 8; i++) {
      const key = row['StatsKey' + i];
      if (key === null || key === undefined) continue;
      out.push({ id: t.statId[key], min: row['Stat' + i + 'Min'], max: row['Stat' + i + 'Max'] });
    }
    return out;
  };

  const specs = [], report = [];
  for (const item of parseItems(xml)) {
    if (item.rarity === 'UNIQUE' || item.rarity === 'RELIC') continue;
    const found = findBase(item.lines, item.start, baseByName, t.bases);
    const entry = { id: item.id, rarity: item.rarity, name: found?.name, base: found?.base.Name, mods: [], unmatched: [], influence: [], notes: [], flags: [] };
    if (!found) { entry.error = 'unknown base'; report.push(entry); continue; }
    const base = found.base;
    const props = {};
    const text = [];
    for (let i = found.index + 1; i < item.lines.length; i++) {
      const line = item.lines[i];
      if (line.startsWith('<') || line.startsWith('(')) continue;
      if (FLAGS[line]) { entry.flags.push(FLAGS[line]); continue; }
      const pm = line.match(/^([A-Za-z ]+):\s*(.*)$/);
      if (pm && PROP.has(pm[1].trim())) { props[pm[1].trim()] = pm[2].trim(); continue; }
      text.push(line);
    }
    const influence = text.filter((l) => /^(Shaper|Elder|Crusader|Redeemer|Hunter|Warlord) Item$/.test(l));
    entry.influence = influence.map((l) => l.split(' ')[0].toLowerCase());
    const mods = [];
    // Implicits come from the base itself, so their multi-stat rows are kept whole; the item's text then repeats them,
    // and those repeated lines are covered rather than matched again.
    const explained = new Set();
    for (const key of base.Implicit_ModsKeys ?? []) {
      const row = t.mods[key];
      if (!row) continue;
      const stats = rowStats(row);
      for (const s of stats) explained.add(s.id);
      mods.push({ row, section: 'Implicit', values: stats.map((s) => s.min) });
    }
    for (const line of text) {
      const clean = line.replace(/\{[a-z]+\}/g, '').trim();
      if (!clean || influence.includes(line)) continue;
      if (/^(Allocates |Added Small Passive|Adds \d+ Passive|\d+ Added Passive|Cluster Jewel|Catalyst|Memory Strands|.*BasePercentile|Item sells)/.test(clean)) {
        entry.notes.push('unsupported: ' + clean);
        continue;
      }
      const mapped = mapLine(clean);
      if (!mapped) { entry.unmatched.push({ line: clean, why: 'no rule' }); continue; }
      if (mapped.ids.every((id) => explained.has(id))) {
        entry.notes.push('repeats the base implicit: ' + clean);
        continue;
      }
      let best = null;
      for (const id of mapped.ids) {
        for (const row of byStat.get(id) ?? []) {
          if (row.Domain !== base.ModDomain) continue;
          // Prefix (1) and suffix (2) rows come first: they are the ordinary affixes the text names. Rows of the
          // synthesised/implicit generation (3) are a fallback for stats only those carry, such as a belt's +72 strength.
          if (![1, 2, 3].includes(row.GenerationType)) continue;
          const code = row.CraftingItemClassRestrictions ?? [];
          if (code.length > 0 && !code.includes(base.ItemClassesKey)) continue;
          const stats = rowStats(row);
          const exact = stats.length === mapped.ids.length;
          const values = [];
          let ok = true;
          for (const s of stats) {
            const at = mapped.ids.indexOf(s.id);
            if (at < 0) { if (exact) { ok = false; break; } continue; }
            const v = mapped.values[at];
            if (v < Math.min(s.min, s.max) || v > Math.max(s.min, s.max)) { ok = false; break; }
            values.push(v);
          }
          if (!ok) continue;
          const span = stats.reduce((acc, s) => acc + Math.abs(s.max - s.min), 0);
          const filled = stats.map((s, i) => (mapped.ids.includes(s.id) ? values[mapped.ids.indexOf(s.id)] : s.min));
          const candidate = { row, values: filled, span, exact, ordinary: row.GenerationType !== 3 };
          if (!best
            || (candidate.ordinary && !best.ordinary)
            || (candidate.ordinary === best.ordinary && candidate.exact && !best.exact)
            || (candidate.ordinary === best.ordinary && candidate.exact === best.exact && span < best.span)) best = candidate;
        }
      }
      if (!best) { entry.unmatched.push({ line: clean, why: 'no row' }); continue; }
      // The item's own implicits came from the base above, so a text line is explicit: a synthesised-generation row
      // still counts as an explicit modifier here, which is what the stat does on the character.
      mods.push({ row: best.row, section: 'Explicit', values: best.values });
      if (!best.exact) entry.notes.push('hybrid row, other stats at their minimum: ' + clean);
      if (!best.ordinary) entry.notes.push('synthesised-generation row: ' + clean);
    }
    entry.mods = mods.map((m) => ({ id: m.row.Id, section: m.section, gen: m.row.GenerationType, values: m.values }));
    const quality = props.Quality ? N(props.Quality) : undefined;
    // The command travels through a chat line, so the spec is as short as it can be while staying exact: the base by
    // name, and a section only where the row's own default list would be wrong (a synthesised-generation row used as an
    // explicit modifier, which the server would otherwise put in the implicit list).
    const sectionOf = (m) => (m.gen <= 2 ? 'Explicit' : 'Implicit');
    specs.push({
      Base: base.Name,
      Rarity: item.rarity[0] + item.rarity.slice(1).toLowerCase(),
      ItemLevel: props['Item Level'] ? N(props['Item Level']) : base.DropLevel,
      Quality: quality ?? undefined,
      Mods: entry.mods.map((m) => ({
        Id: m.id,
        ...(sectionOf(m) === m.section ? {} : { Section: m.section }),
        Values: m.values,
      })),
    });
    report.push(entry);
  }
  await fs.writeFile(outFile, JSON.stringify(specs, null, 2), 'utf8');
  const commands = specs.map((s) => '/item_give ' + JSON.stringify(s));
  const commandFile = path.join('artifacts', 'pob-item-commands.txt');
  await fs.writeFile(commandFile, commands.join('\n') + '\n', 'utf8');
  return { specs, report, commands };
}

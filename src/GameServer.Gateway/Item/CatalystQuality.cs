using System.Text.Json;
using GameServer.Core.Protocol.Instance;

/// <summary>Effective catalyst values, leaving saved rolls and wire values unchanged.</summary>
sealed class CatalystQuality(string folder)
{
    sealed record Rule(HashSet<string> Tags, bool Explicit, bool Implicit, bool Prefix, bool Suffix);
    readonly Lazy<Dictionary<byte, Rule>> rules = new(() => Load(folder));
    readonly Lazy<HashSet<uint>> jewellery = new(() => LoadJewellery(folder));

    public int Value(Item item, ItemMod mod, ModCatalog.Mod row, int index)
    {
        int value = mod.Values[index];
        if (item.Quality == 0 || item.QualityType == byte.MaxValue || !row.Stats[index].Scalable
            || !jewellery.Value.Contains(item.TypeHash) || !rules.Value.TryGetValue(item.QualityType, out var rule)) return value;
        bool section = mod.Section == ModSection.Implicit ? rule.Implicit
            : mod.Section == ModSection.Explicit && rule.Explicit;
        if (!section || rule.Prefix && row.GenerationType != 1 || rule.Suffix && row.GenerationType != 2
            || rule.Tags.Count > 0 && !rule.Tags.Overlaps(row.Tags)) return value;
        // Magnitude rounds toward zero for both signs, once per raw stat, before summing items.
        return (int)Math.Clamp((long)value * (100 + item.Quality) / 100, int.MinValue, int.MaxValue);
    }

    static Dictionary<byte, Rule> Load(string folder)
    {
        string quality = Path.Combine(folder, "tables", "AlternateQualityTypes.json"),
            effects = Path.Combine(folder, "tables", "ModEffectStats.json"), tags = Path.Combine(folder, "tables", "Tags.json");
        if (!File.Exists(quality) || !File.Exists(effects) || !File.Exists(tags)) return [];
        using var q = GameTables.Open(quality);
        using var e = GameTables.Open(effects);
        using var t = GameTables.Open(tags);
        var names = t.RootElement.EnumerateArray().Select(r => r.GetProperty("Id").GetString() ?? "").ToArray();
        var rows = e.RootElement.EnumerateArray().ToArray();
        var result = new Dictionary<byte, Rule>();
        int index = 0;
        foreach (var r in q.RootElement.EnumerateArray())
        {
            int current = index++;
            if (current >= byte.MaxValue || !(r.GetProperty("Id").GetString() ?? "").StartsWith("Jewellery", StringComparison.Ordinal)
                || !r.GetProperty("ModEffectStat").TryGetInt32(out int key) || key < 0 || key >= rows.Length) continue;
            var rule = rows[key];
            result[(byte)current] = new(
                rule.GetProperty("TagsKeys").EnumerateArray().Select(v => v.GetInt32())
                    .Where(k => k >= 0 && k < names.Length).Select(k => names[k]).ToHashSet(StringComparer.Ordinal),
                rule.GetProperty("ApplyToExplicit").GetBoolean(), rule.GetProperty("ApplyToImplicit").GetBoolean(),
                rule.GetProperty("ApplyToPrefix").GetBoolean(), rule.GetProperty("ApplyToSuffix").GetBoolean());
        }
        return result;
    }

    static HashSet<uint> LoadJewellery(string folder)
    {
        string bases = Path.Combine(folder, "tables", "BaseItemTypes.json"), classes = Path.Combine(folder, "tables", "ItemClasses.json");
        if (!File.Exists(bases) || !File.Exists(classes)) return [];
        using var b = GameTables.Open(bases);
        using var c = GameTables.Open(classes);
        var names = c.RootElement.EnumerateArray().Select(r => r.GetProperty("Id").GetString()).ToArray();
        return b.RootElement.EnumerateArray().Where(r => r.GetProperty("ItemClassesKey").TryGetInt32(out int k)
                && k >= 0 && k < names.Length && names[k] is "Amulet" or "Ring" or "Belt")
            .Select(r => r.GetProperty("HASH32").GetUInt32()).ToHashSet();
    }
}

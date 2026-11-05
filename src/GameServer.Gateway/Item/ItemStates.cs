using GameServer.Core.Protocol.Instance;

/// <summary>What an item is besides its mods, for /item_state: the states Agents/docs/protocol/13-items.md §2 places in
/// the Base and Mods components. Currency that creates them in play is separate work.</summary>
static class ItemStates
{
    /// <summary>The item with a state switched, or null when there is no such state. "clear" removes them all.
    /// <paramref name="mods"/> lets corruption raise a Foulborn item's Mutated modifiers (see <see cref="Corrupt"/>).</summary>
    public static Item? Toggle(Item item, string state, ModCatalog? mods = null) => state.ToLowerInvariant() switch
    {
        "corrupted" => Corrupt(item, mods),
        "mirrored" => item with { Mirrored = !item.Mirrored },
        "split" => item with { Split = !item.Split },
        "synthesised" or "synthesized" => item with { Synthesised = !item.Synthesised },
        "foulborn" or "mutated" => item with { Foulborn = !item.Foulborn },
        "clear" => item with
        {
            Corrupted = false, Mirrored = false, Split = false, Synthesised = false, Foulborn = false, Influences = ItemInfluence.None,
        },
        _ => char.IsLetter(state[0]) && Enum.TryParse<ItemInfluence>(state, true, out var influence)
            && influence != ItemInfluence.None ? item with { Influences = item.Influences ^ influence } : null,
    };

    /// <summary>The Foulborn Rathpith Globe modifiers whose corrupted values are one above the generated 3%: its
    /// damage over time per 100 life (the Foulborn replacement) and its spell damage per 100 life.</summary>
    static readonly string[] CorruptedFoulbornRaises =
    [
        "MutatedUniqueShieldInt1DamageOverTimePer100PlayerMaxLife",
        "SpellDamagePerLifeUnique__1",
    ];

    /// <summary>Corruption raises the two Foulborn Rathpith Globe modifiers above from 3% to 4%: the corrupted
    /// page's numbers are one above the generated ones. Switching corruption on does it; switching it off keeps the
    /// raised values, and every other item is untouched.</summary>
    public static Item Corrupt(Item item, ModCatalog? mods)
    {
        var corrupted = item with { Corrupted = !item.Corrupted };
        if (item.Corrupted || !item.Foulborn || mods is null) return corrupted;
        return corrupted with
        {
            Mods = [.. corrupted.Mods.Select(m => mods.ByHash(m.Hash) is { } row
                && Array.IndexOf(CorruptedFoulbornRaises, row.Id) >= 0
                ? m with { Values = [.. m.Values.Select(v => v + 1)] }
                : m)],
        };
    }

    /// <summary>The state lives in the Mods component; the others are in Base, which every item has.</summary>
    public static bool NeedsMods(string state) =>
        state.ToLowerInvariant() is "mirrored" or "split" or "synthesised" or "synthesized" or "foulborn" or "mutated";

    public static string Describe(Item item)
    {
        var states = new List<string>();
        if (item.Corrupted) states.Add("corrupted");
        if (item.Mirrored) states.Add("mirrored");
        if (item.Split) states.Add("split");
        if (item.Synthesised) states.Add("synthesised");
        if (item.Foulborn) states.Add("foulborn");
        states.AddRange(Enum.GetValues<ItemInfluence>().Where(i => i != ItemInfluence.None && item.Influences.HasFlag(i))
            .Select(i => i.ToString().ToLowerInvariant()));
        return states.Count == 0 ? "No item states" : "Item states: " + string.Join(", ", states);
    }
}

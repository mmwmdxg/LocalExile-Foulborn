using GameServer.Core.Protocol.Instance;

namespace GameServer.Gateway.Tests;

public class VaalUniqueTests
{
    static readonly ItemCatalog.BaseType Body = Program.Items.Find("Saintly Chainmail")!;
    static ItemRolls Rolls => new(Program.Items);

    static ModCatalog.Mod Mod(string id) => Program.Items.Mods.All.Single(m => m.Id == id);

    static Item Unique(ItemCatalog.BaseType type) =>
        ItemCatalog.Create(type) with
        {
            Rarity = ItemRarity.Unique,
            NameWords = [],
            Mods = [.. ItemCatalog.Create(type).Mods.Where(m => m.Section == ModSection.Implicit),
                    Mod("IntelligenceUnique__17").Roll(ModSection.Explicit, new Random(2))],
        };

    [Fact]
    public void Vaal_corrupts_a_unique_and_keeps_its_modifiers()
    {
        var unique = Unique(Body);
        var before = unique.Mods.ToList();

        var result = Rolls.Vaal(unique, Body, (0, 0, 0), new Random(1));

        Assert.NotNull(result);
        Assert.True(result!.Corrupted);
        Assert.True(result.Identified);
        Assert.Equal(ItemRarity.Unique, result.Rarity);
        Assert.Empty(result.NameWords);
        // No item-domain Mods.dat GenerationType 5 row has a spawn weight, so the unique only takes the corrupted
        // state; the local rule must not fabricate a corruption implicit.
        Assert.Equal(before, result.Mods);
    }

    [Fact]
    public void Vaal_still_refuses_a_corrupted_or_mirrored_unique_and_non_equipment()
    {
        var unique = Unique(Body);
        Assert.Null(Rolls.Vaal(unique with { Corrupted = true }, Body, (0, 0, 0), new Random(1)));
        Assert.Null(Rolls.Vaal(unique with { Mirrored = true }, Body, (0, 0, 0), new Random(1)));

        var gemType = Program.Items.Find("Metadata/Items/Gems/SkillGemFireball")!;
        var gem = ItemCatalog.Create(gemType) with { Rarity = ItemRarity.Unique };
        Assert.Null(Rolls.Vaal(gem, gemType, (0, 0, 0), new Random(1)));
    }
}

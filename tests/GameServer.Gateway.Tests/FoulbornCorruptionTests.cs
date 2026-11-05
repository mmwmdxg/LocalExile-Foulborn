using GameServer.Core.Protocol.Instance;

namespace GameServer.Gateway.Tests;

// Corrupting the Foulborn Rathpith Globe raises its two per-100-life modifiers from 3% to 4% (the corrupted page's
// numbers); every other item keeps its values.
public sealed class FoulbornCorruptionTests
{
    [Fact]
    public void Corruption_raises_only_the_foulborn_rathpith_life_modifiers()
    {
        var items = new ItemCatalog("data");
        var dot = items.Mods.ById("MutatedUniqueShieldInt1DamageOverTimePer100PlayerMaxLife");
        var spell = items.Mods.ById("SpellDamagePerLifeUnique__1");
        var plain = items.Mods.ById("MaximumLifeShieldInt1");
        Assert.NotNull(dot);
        Assert.NotNull(spell);
        Assert.NotNull(plain);
        var item = new Item(1, ["Armour", "Mods"], 0) with
        {
            Foulborn = true,
            Mods =
            [
                new ItemMod(ModSection.Explicit, dot!.Hash, [3]),
                new ItemMod(ModSection.Explicit, spell!.Hash, [3]),
                new ItemMod(ModSection.Explicit, plain!.Hash, [9]),
            ],
        };

        var corrupted = ItemStates.Corrupt(item, items.Mods);

        Assert.True(corrupted.Corrupted);
        Assert.Equal(4, corrupted.Mods[0].Values[0]);
        Assert.Equal(4, corrupted.Mods[1].Values[0]);
        Assert.Equal(9, corrupted.Mods[2].Values[0]);

        // The same modifier without the Foulborn flag keeps its generated value.
        var plainItem = new Item(1, ["Armour", "Mods"], 0) with
        {
            Mods = [new ItemMod(ModSection.Explicit, dot.Hash, [3])],
        };
        Assert.Equal(3, ItemStates.Corrupt(plainItem, items.Mods).Mods[0].Values[0]);
    }
}

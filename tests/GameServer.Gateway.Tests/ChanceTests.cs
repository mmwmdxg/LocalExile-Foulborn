using GameServer.Core.Protocol.Instance;

namespace GameServer.Gateway.Tests;

public class ChanceTests
{
    [Fact]
    public void Chance_turns_normal_equipment_into_its_bases_unique_by_weight()
    {
        var uniques = new UniqueCatalog(Program.Items, "data");
        var definition = uniques.All.First(d => !d.Corrupted && !d.Name.StartsWith("Replica ")
            && Program.Items.Find(d.BaseTypeId) is { Supported: true } t
            && t.Class is "Ring" or "Amulet" or "Belt" or "Body Armour" or "Shield" or "Helmet" or "Gloves" or "Boots");
        var baseType = Program.Items.Find(definition.BaseTypeId)!;
        var item = ItemCatalog.Create(baseType) with { Rarity = ItemRarity.Normal, ItemLevel = 70 };

        // Only this unique carries weight, so it is certain and it is the one picked.
        var unique = uniques.Chance(item, name => name == definition.Name ? 1 : 0, new Random(3));
        Assert.NotNull(unique);
        Assert.Equal(ItemRarity.Unique, unique!.Rarity);
        Assert.Equal(item.TypeHash, unique.TypeHash);
        Assert.Equal(70, unique.ItemLevel);

        // A rare item is accepted too, and its enchantment survives: the Tailoring Orb's Heist enchant is what a
        // rare body armour is made for, so chancing it into a unique must keep that one modifier.
        var enchant = Program.Items.Mods.ById("ArmourEnchantmentHeistLifeEffectSocketPenalty1")!;
        var rare = item with
        {
            Rarity = ItemRarity.Rare,
            Mods = [new ItemMod(ModSection.Enchant, enchant.Hash, [15, -3])],
        };
        var fromRare = uniques.Chance(rare, name => name == definition.Name ? 1 : 0, new Random(3));
        Assert.NotNull(fromRare);
        Assert.Equal(ItemRarity.Unique, fromRare!.Rarity);
        Assert.Contains(fromRare.Mods, m => m.Section == ModSection.Enchant && m.Hash == enchant.Hash);
        var noUniques = Program.Items.All.First(t => t.Supported
            && t.Class is "Ring" or "Amulet" or "Belt" or "Body Armour" or "Shield" or "Helmet" or "Gloves" or "Boots"
            && !uniques.All.Any(d => Program.Items.Find(d.BaseTypeId)?.Hash == t.Hash));
        Assert.Null(uniques.Chance(ItemCatalog.Create(noUniques) with { Rarity = ItemRarity.Normal }, _ => 1, new Random(3)));
    }
}

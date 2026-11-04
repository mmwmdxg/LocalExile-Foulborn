using GameServer.Core.Protocol.Instance;

namespace GameServer.Gateway.Tests;

public class TailoringTests
{
    static readonly ItemCatalog.BaseType Body = Program.Items.Find("Glorious Plate")!;
    static readonly ItemCatalog.BaseType Ring = Program.Items.Find("Iron Ring")!;

    static ModCatalog.Mod Mod(string id) => Program.Items.Mods.All.Single(m => m.Id == id);

    static Item Armour() =>
        ItemCatalog.Create(Body, [new ItemSocket(SocketColour.Red, ItemCatalog.Create(Program.Items.Find("Fireball")!))]) with
        {
            Rarity = ItemRarity.Rare,
            ItemLevel = 86,
            Mods =
            [
                Mod("IncreasedLife9").Roll(ModSection.Explicit, new Random(1)),
                Mod("EnchantmentRighteousFireRadius2").Roll(ModSection.Enchant, new Random(1)),
            ],
        };

    [Fact]
    public void Tailoring_replaces_the_enchantment_and_keeps_the_other_modifiers()
    {
        var item = Armour();
        var result = CurrencyEffects.Tailoring(item, Body, Program.Items.Mods, Program.Items, new Random(7));

        Assert.NotNull(result);
        var enchant = Assert.Single(result!.Mods, m => m.Section == ModSection.Enchant);
        var enchantMod = Program.Items.Mods.All.Single(m => m.Hash == enchant.Hash);
        Assert.StartsWith("ArmourEnchantmentHeist", enchantMod.Id);

        Assert.Equal(item.Mods.Count(m => m.Section != ModSection.Enchant),
            result.Mods.Count(m => m.Section != ModSection.Enchant));
        Assert.Equal(item.Quality, result.Quality);
        Assert.Equal(item.Sockets, result.Sockets);
        Assert.Equal(item.Rarity, result.Rarity);
    }

    [Fact]
    public void Tailoring_refuses_non_body_armour_and_corrupted_or_mirrored_targets()
    {
        Assert.Null(CurrencyEffects.Tailoring(Armour() with { Corrupted = true }, Body, Program.Items.Mods, Program.Items, new Random(1)));
        Assert.Null(CurrencyEffects.Tailoring(Armour() with { Mirrored = true }, Body, Program.Items.Mods, Program.Items, new Random(1)));

        var ring = ItemCatalog.Create(Ring) with { Rarity = ItemRarity.Rare };
        Assert.Null(CurrencyEffects.Tailoring(ring, Ring, Program.Items.Mods, Program.Items, new Random(1)));

        var gemType = Program.Items.Find("Metadata/Items/Gems/SkillGemFireball")!;
        Assert.Null(CurrencyEffects.Tailoring(ItemCatalog.Create(gemType), gemType, Program.Items.Mods, Program.Items, new Random(1)));
    }
}

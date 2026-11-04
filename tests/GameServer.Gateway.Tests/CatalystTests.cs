using GameServer.Core.Protocol.Instance;

namespace GameServer.Gateway.Tests;

public class CatalystTests
{
    static readonly ItemCatalog.BaseType Amulet = Program.Items.Find("Onyx Amulet")!;
    static readonly ItemCatalog.BaseType Body = Program.Items.Find("Glorious Plate")!;
    const string Intrinsic = "Metadata/Items/Currency/CurrencyJewelleryQualityAttribute";
    const string Prismatic = "Metadata/Items/Currency/CurrencyJewelleryQualityResistance";

    static Item Item(ItemCatalog.BaseType type, byte quality = 0, byte qualityType = byte.MaxValue) =>
        ItemCatalog.Create(type) with { Rarity = ItemRarity.Rare, Quality = quality, QualityType = qualityType };

    [Fact]
    public void Catalyst_sets_the_quality_type_and_raises_quality_to_the_cap()
    {
        Assert.Equal((byte)6, CurrencyEffects.CatalystQualityType(Intrinsic));

        var once = CurrencyEffects.Catalyst(Item(Amulet), Amulet, 6, new Random(1));
        Assert.NotNull(once);
        Assert.Equal(1, once!.Quality);
        Assert.Equal(6, once.QualityType);

        var twice = CurrencyEffects.Catalyst(once, Amulet, 6, new Random(1));
        Assert.Equal(2, twice!.Quality);

        Assert.Null(CurrencyEffects.Catalyst(Item(Amulet, 20, 6), Amulet, 6, new Random(1)));
    }

    [Fact]
    public void Catalyst_refuses_another_type_non_jewellery_and_corrupted_targets()
    {
        Assert.Equal((byte)5, CurrencyEffects.CatalystQualityType(Prismatic));
        Assert.Null(CurrencyEffects.CatalystQualityType("Metadata/Items/Currency/CurrencyCorrupt"));
        Assert.Null(CurrencyEffects.CatalystQualityType("Metadata/Items/Currency/CurrencyJewelleryQualityVaal"));

        Assert.Null(CurrencyEffects.Catalyst(Item(Amulet, 3, 6), Amulet, 5, new Random(1)));
        Assert.Null(CurrencyEffects.Catalyst(Item(Body), Body, 6, new Random(1)));
        Assert.Null(CurrencyEffects.Catalyst(Item(Amulet) with { Corrupted = true }, Amulet, 6, new Random(1)));
        Assert.Null(CurrencyEffects.Catalyst(Item(Amulet) with { Mirrored = true }, Amulet, 6, new Random(1)));

        // The Tainted Catalyst is the reverse: corrupted jewellery only, and it picks a type when the item has none.
        Assert.Equal("Metadata/Items/Currency/CurrencyJewelleryQualityVaal", CurrencyEffects.TaintedCatalyst);
        Assert.Null(CurrencyEffects.Catalyst(Item(Amulet, 3), Amulet, 0, new Random(1), tainted: true));
        var tainted = CurrencyEffects.Catalyst(Item(Amulet, 3) with { Corrupted = true }, Amulet, 0, new Random(1), tainted: true);
        Assert.NotNull(tainted);
        Assert.Equal(4, tainted!.Quality);
        Assert.InRange(tainted.QualityType, (byte)0, (byte)9);
        var kept = CurrencyEffects.Catalyst(Item(Amulet, 3, 5) with { Corrupted = true }, Amulet, 0, new Random(1), tainted: true);
        Assert.Equal(5, kept!.QualityType);
    }
}

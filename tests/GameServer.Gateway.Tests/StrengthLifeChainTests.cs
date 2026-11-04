using GameServer.Core.Protocol.Instance;

namespace GameServer.Gateway.Tests;

public class StrengthLifeChainTests
{
    [Fact]
    public void Attribute_quality_raises_strength_then_maximum_life_then_righteous_fire_base()
    {
        var mods = Program.Items.Mods;
        var strength = mods.ById("Strength1")!;
        var allAttributes = mods.ById("AllAttributesImplicitAmulet1")!;
        Item Amulet(byte quality) => ItemCatalog.Create(Program.Items.Find("Onyx Amulet")!) with
        {
            Quality = quality, QualityType = 6,
            Mods = [new(ModSection.Explicit, strength.Hash, [37]),
                new(ModSection.Implicit, allAttributes.Hash, [16])]
        };

        (uint Life, int Strength) Pools(byte quality)
        {
            var raw = ItemModStats.RawAttributes([Amulet(quality)], mods);
            int str = (int)raw.GetValueOrDefault("additional_strength");
            int all = (int)raw.GetValueOrDefault("additional_all_attributes");
            var bonus = new CharacterStats.Bonus(Strength: str + all, Dexterity: all, Intelligence: all);
            var pools = CharacterStats.For(SkillCatalog.ClassInfo.Witch, 1, bonus);
            return (pools.Life, pools.Strength);
        }

        var plain = Pools(0);
        var catalysed = Pools(20);

        // 37 strength and 16 all attributes become 44 and 19 at 20% attribute quality; the Witch's own 14 add on.
        Assert.Equal(67, plain.Strength);
        Assert.Equal(77, catalysed.Strength);
        // Strength grants one life per two points: 33 and 38, on the 50 a level-one Witch has before it.
        Assert.Equal(83u, plain.Life);
        Assert.Equal(88u, catalysed.Life);

        // The burning's base share of maximum life per minute, before any increase.
        Assert.Equal(83 * 4200 / 6000.0, RighteousFireDamage.PerSecond(plain.Life, 0, 4200, 4200, _ => 0), 6);
        Assert.Equal(88 * 4200 / 6000.0, RighteousFireDamage.PerSecond(catalysed.Life, 0, 4200, 4200, _ => 0), 6);
    }
}

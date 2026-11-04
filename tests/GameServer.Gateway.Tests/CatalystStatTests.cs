using GameServer.Core.Protocol.Instance;

namespace GameServer.Gateway.Tests;

public class CatalystStatTests
{
    [Fact]
    public void Typed_quality_scales_shared_attributes_and_resistance_without_changing_saved_rolls()
    {
        var mods = Program.Items.Mods;
        var strength = mods.ById("Strength1")!;
        var implicitMod = mods.ById("AllAttributesImplicitAmulet1")!;
        var resistance = mods.ById("FireResist1")!;
        var item = ItemCatalog.Create(Program.Items.Find("Onyx Amulet")!) with
        {
            Quality = 20, QualityType = 6,
            Mods = [new(ModSection.Explicit, strength.Hash, [37]),
                new(ModSection.Implicit, implicitMod.Hash, [16]), new(ModSection.Explicit, resistance.Hash, [31])]
        };
        var sum = ItemModStats.Sum([item], mods);
        Assert.Equal(44, sum["additional_strength"]);
        Assert.Equal(19, sum["additional_all_attributes"]);
        Assert.Equal(31, sum["base_fire_damage_resistance_%"]);
        Assert.Equal(44, ItemModStats.RawAttributes([item], mods)["additional_strength"]);
        Assert.Equal(44, ItemModStats.Global([item], mods, Program.Stats)[Program.Stats.Key("additional_strength")!.Value]);
        Assert.Equal(37, item.Mods[0].Values[0]);
        Assert.Equal(44, ItemModStats.Sum([item], mods)["additional_strength"]);
        Assert.Equal(37, ItemModStats.Sum([item with { Quality = 0 }], mods)["additional_strength"]);
        Assert.Equal(37, ItemModStats.Sum([item with { QualityType = 255 }], mods)["additional_strength"]);
        Assert.Equal(37, ItemModStats.Sum([item with { QualityType = 254 }], mods)["additional_strength"]);
        Assert.Equal(37, ItemModStats.Sum([item with { QualityType = 5 }], mods)["base_fire_damage_resistance_%"]);
        Assert.Equal(37, ItemModStats.Sum([item with { QualityType = 5 }], mods)["additional_strength"]);
        var negative = item with { Mods = [new(ModSection.Explicit, strength.Hash, [-37])] };
        Assert.Equal(-44, ItemModStats.Sum([negative], mods)["additional_strength"]);
        var body = item with { TypeHash = Program.Items.Find("Glorious Plate")!.Hash };
        Assert.Equal(37, ItemModStats.Sum([body], mods)["additional_strength"]);
        Assert.Equal(37, ItemModStats.Sum([item with { QualityType = 10 }], mods)["additional_strength"]);
    }

    [Fact]
    public void Catalyst_rules_match_tags_sections_affix_sides_and_scalability()
    {
        var mods = Program.Items.Mods;
        var item = ItemCatalog.Create(Program.Items.Find("Onyx Amulet")!) with { Quality = 20 };
        string[] tags = ["elemental_damage", "caster", "attack", "defences", "resource", "resistance", "attribute",
            "physical_damage", "speed", "critical"];
        for (byte type = 0; type < tags.Length; type++)
        {
            var row = new ModCatalog.Mod(0, "test", 1, "", 1, 1, 1,
                [new("test", 0, 100), new("switch", 0, 100, Scalable: false)])
                { Tags = new HashSet<string> { tags[type] } };
            var mod = new ItemMod(ModSection.Explicit, 1, [37, 37]);
            var typed = item with { QualityType = type };
            Assert.Equal(44, mods.CatalystQuality.Value(typed, mod, row, 0));
            Assert.Equal(37, mods.CatalystQuality.Value(typed, mod, row, 1));
            Assert.Equal(44, mods.CatalystQuality.Value(typed, mod with { Section = ModSection.Implicit }, row, 0));
            foreach (var section in new[] { ModSection.Enchant, ModSection.Scourge, ModSection.Hidden })
                Assert.Equal(37, mods.CatalystQuality.Value(typed, mod with { Section = section }, row, 0));
            Assert.Equal(37, mods.CatalystQuality.Value(typed, mod, row with { Tags = new HashSet<string>() }, 0));
            if (type == 7)
                Assert.Equal(44, mods.CatalystQuality.Value(typed, mod, row with { Tags = new HashSet<string> { "chaos_damage", "physical_damage" } }, 0));
        }
        var prefix = mods.ById("IncreasedLife1")!;
        var suffix = mods.ById("Strength1")!;
        foreach (byte type in new byte[] { 16, 17 })
            foreach (var row in new[] { prefix, suffix })
            {
                var mod = new ItemMod(ModSection.Explicit, row.Hash, [37]);
                int expected = row.GenerationType == (type == 16 ? 1 : 2) ? 44 : 37;
                Assert.Equal(expected, mods.CatalystQuality.Value(item with { QualityType = type }, mod, row, 0));
                Assert.Equal(37, mods.CatalystQuality.Value(item with { QualityType = type }, mod with { Section = ModSection.Implicit }, row, 0));
            }
        var local = mods.All.First(m => m.Tags.Contains("defences") && m.Stats.Any(s => s.Local && s.Scalable));
        int index = Enumerable.Range(0, local.Stats.Count).First(i => local.Stats[i].Local && local.Stats[i].Scalable);
        var localItem = item with { QualityType = 3, Mods = [new(ModSection.Explicit, local.Hash, Enumerable.Repeat(37, local.Stats.Count).ToArray())] };
        int copies = local.Stats.Count(s => s.Local && s.Id == local.Stats[index].Id);
        Assert.Equal(44 * copies, ItemModStats.Local(localItem, mods)[local.Stats[index].Id]);
    }
}

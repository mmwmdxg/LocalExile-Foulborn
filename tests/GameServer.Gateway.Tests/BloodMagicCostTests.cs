namespace GameServer.Gateway.Tests;

public class BloodMagicCostTests
{
    [Fact]
    public void Blood_magic_pays_skills_from_life_and_never_takes_the_last_point()
    {
        uint life = 1000, mana = 200;

        // Without the keystone the cost comes from mana and life is untouched.
        Assert.True(Game.PaySkillCost(false, 50, ref life, ref mana));
        Assert.Equal((1000u, 150u), (life, mana));
        Assert.False(Game.PaySkillCost(false, 300, ref life, ref mana));
        Assert.Equal((1000u, 150u), (life, mana));

        // With it the same cost comes from life and mana stays.
        Assert.True(Game.PaySkillCost(true, 50, ref life, ref mana));
        Assert.Equal((950u, 150u), (life, mana));

        // A cost that would leave no life is refused, and nothing is taken.
        Assert.False(Game.PaySkillCost(true, 950, ref life, ref mana));
        Assert.Equal((950u, 150u), (life, mana));

        // A free skill always passes.
        Assert.True(Game.PaySkillCost(true, 0, ref life, ref mana));
        Assert.Equal((950u, 150u), (life, mana));
    }
}

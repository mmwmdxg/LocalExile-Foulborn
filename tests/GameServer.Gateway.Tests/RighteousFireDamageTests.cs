namespace GameServer.Gateway.Tests;

public class RighteousFireDamageTests
{
    static Func<string, int> Getter(Dictionary<string, int> stats) => stat => stats.GetValueOrDefault(stat);

    // 5,000 life at 42% per second is 3,500 damage per second before modifiers.
    static double Base => 5000 * 4200 / 6000.0;

    [Fact]
    public void Burning_fire_area_increases_add_and_ignite_or_missing_keys_do_not_apply()
    {
        var stats = new Dictionary<string, int>
        {
            ["damage_+%"] = 20,
            ["fire_damage_+%"] = 30,
            ["elemental_damage_+%"] = 10,
            ["damage_over_time_+%"] = 20,
            ["fire_damage_over_time_+%"] = 10,
            ["burn_damage_+%"] = 10,
            ["area_damage_+%"] = 50,
            // Not the burning's: ignite damage and the Id an older revision read, which Stats.dat has no row for.
            ["ignite_damage_+%"] = 500,
            ["area_damage_+"] = 500,
        };

        Assert.Equal(Base * 2.5, RighteousFireDamage.PerSecond(5000, 0, 4200, 4200, Getter(stats)), 6);
    }

    [Fact]
    public void Dot_multipliers_and_final_mores_multiply_after_the_increases()
    {
        var stats = new Dictionary<string, int>
        {
            ["area_damage_+%"] = 50,
            ["fire_dot_multiplier_+"] = 20,
            ["dot_multiplier_+"] = 10,
            ["support_burning_damage_+%_final"] = 25,
            ["support_area_concentrate_area_damage_+%_final"] = 30,
        };

        // 5,000 life and 1,000 energy shield at 42% per minute each: 4,200 per second.
        const double basePerSecond = (5000 * 4200 + 1000 * 4200) / 6000.0;
        Assert.Equal(basePerSecond * 1.5 * 1.3 * 1.25 * 1.3,
            RighteousFireDamage.PerSecond(5000, 1000, 4200, 4200, Getter(stats)), 6);
    }
}

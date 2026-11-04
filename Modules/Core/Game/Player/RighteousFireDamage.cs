/// <summary>
/// Righteous Fire's damage to enemies before the target's mitigation: its percentages of maximum life and energy shield
/// per minute, then the character's and the supports' increases, damage-over-time multipliers and *_final multipliers.
/// The names are 3.29.3b Stats.dat Ids: the burning is fire, elemental, burning damage over time in an area, so those
/// increases add. `area_damage_+%` is the stat's real Id; an older revision read `area_damage_+`, which no row has.
/// </summary>
static class RighteousFireDamage
{
    /// <summary>The increases that add together before the multipliers.</summary>
    internal static readonly string[] Increases =
        ["damage_+%", "fire_damage_+%", "elemental_damage_+%", "damage_over_time_+%", "fire_damage_over_time_+%",
         "burn_damage_+%", "area_damage_+%"];

    /// <summary>The additive damage-over-time multipliers.</summary>
    internal static readonly string[] Multipliers = ["fire_dot_multiplier_+", "dot_multiplier_+"];

    /// <summary>The *_final multipliers, each applied once (supports included, Burning Damage among them).</summary>
    internal static readonly string[] Mores =
        ["active_skill_fire_damage_+%_final", "active_skill_elemental_damage_+%_final",
         "combined_fire_damage_+%_final", "combined_elemental_damage_+%_final",
         "combined_all_damage_over_time_+%_final", "combined_elemental_damage_over_time_+%_final",
         "combined_fire_damage_over_time_+%_final", "support_area_concentrate_area_damage_+%_final",
         "support_burning_damage_+%_final"];

    /// <summary>The burning's damage per second for one tick.</summary>
    internal static double PerSecond(long maximumLife, long maximumEnergyShield, int lifePctPerMinute,
        int energyShieldPctPerMinute, Func<string, int> get)
    {
        double perSecond = (maximumLife * (double)lifePctPerMinute
            + maximumEnergyShield * (double)energyShieldPctPerMinute) / 6000.0;
        double increased = 0;
        foreach (string stat in Increases) increased += get(stat);
        perSecond *= Math.Max(0, 1 + increased / 100.0);
        double multiplier = 0;
        foreach (string stat in Multipliers) multiplier += get(stat);
        perSecond *= Math.Max(0, 1 + multiplier / 100.0);
        foreach (string stat in Mores) perSecond *= Math.Max(0, 1 + get(stat) / 100.0);
        return perSecond;
    }
}

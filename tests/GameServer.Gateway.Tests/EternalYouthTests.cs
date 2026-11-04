namespace GameServer.Gateway.Tests;

public class EternalYouthTests
{
    static Func<string, int> Getter(Dictionary<string, int> stats) => stat => stats.GetValueOrDefault(stat);

    [Fact]
    public void Eternal_youth_sends_the_energy_shield_recharge_to_life()
    {
        // Without the keystone the shield keeps its recharge and life gets none.
        Assert.Equal(20000, EnergyShield.RechargePerMinute(1000, Getter([])), 6);
        Assert.Equal(0, EnergyShield.LifeRechargePerMinute(1000, Getter([])), 6);

        // With it the shield stops recharging and life recharges at the shield's own rate.
        var youth = new Dictionary<string, int> { ["keystone_eternal_youth"] = 1 };
        Assert.Equal(0, EnergyShield.RechargePerMinute(1000, Getter(youth)), 6);
        Assert.Equal(20000, EnergyShield.LifeRechargePerMinute(1000, Getter(youth)), 6);

        // The recharge's rate modifiers reach the moved recharge.
        youth["energy_shield_recharge_rate_+%"] = 50;
        Assert.Equal(30000, EnergyShield.LifeRechargePerMinute(1000, Getter(youth)), 6);

        // A prohibition still stops it, as it stops the shield's.
        youth["cannot_recharge_energy_shield"] = 1;
        Assert.Equal(0, EnergyShield.LifeRechargePerMinute(1000, Getter(youth)), 6);
    }
}

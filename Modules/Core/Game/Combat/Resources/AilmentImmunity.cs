static partial class Game
{
    /// <summary>Ailment immunity with the "while at maximum Endurance Charges" variants folded in: the player's
    /// cannot_be_ignited / cannot_be_shocked stats read as immunity while the endurance charges are full, so the
    /// existing PlayerElementalAilments checks (Reconcile, Inflict) see it without new signatures.</summary>
    internal static Func<string, int> WithEnduranceAilmentImmunity(ClientState conn, Func<string, int> get)
    {
        bool atMax;
        lock (conn.Monsters)
        {
            var session = ChargesOf(conn);
            var limits = session.Limits(ChargeType.Endurance);
            atMax = limits.Maximum > 0 && session.Effective(ChargeType.Endurance) >= limits.Maximum;
        }
        if (!atMax) return get;
        return name => name switch
        {
            "cannot_be_ignited" => get(name) != 0 || get("cannot_be_ignited_while_at_maximum_endurance_charges") != 0 ? 1 : 0,
            "cannot_be_shocked" => get(name) != 0 || get("cannot_be_shocked_while_at_maximum_endurance_charges") != 0 ? 1 : 0,
            _ => get(name),
        };
    }
}

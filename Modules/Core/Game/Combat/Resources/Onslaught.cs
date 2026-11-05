using System.Runtime.CompilerServices;

static partial class Game
{
    // Onslaught: the client icon is BuffDefinitions 215; the server applies the +20% action speed here. Only the
    // "while at maximum endurance charges" grant is modelled so far.
    sealed class OnslaughtState { public double Until; }

    const double KeepSeconds = 4;

    static readonly ConditionalWeakTable<ClientState, OnslaughtState> Onslaughts = new();

    internal static bool OnslaughtActive(ClientState conn, double now) =>
        Onslaughts.TryGetValue(conn, out var state) && now < state.Until;

    /// <summary>Refresh the state from the character's stats: the grant keeps it alive while the endurance charges
    /// are full, and it lapses a few seconds after the condition stops.</summary>
    internal static void RefreshOnslaught(ClientState conn, Func<string, int> get, double now)
    {
        bool granted;
        lock (conn.Monsters)
        {
            var session = ChargesOf(conn);
            var limits = session.Limits(ChargeType.Endurance);
            bool atMax = limits.Maximum > 0 && session.Effective(ChargeType.Endurance) >= limits.Maximum;
            granted = atMax && get("gain_onslaught_while_at_maximum_endurance_charges") != 0;
        }
        var state = Onslaughts.GetOrCreateValue(conn);
        state.Until = granted ? now + KeepSeconds : 0;
    }
}

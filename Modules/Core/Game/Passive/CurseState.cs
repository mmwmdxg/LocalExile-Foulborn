using System.Runtime.CompilerServices;

static partial class Game
{
    // The cursed state the while_cursed modifiers read (dev: /curse, or a monster's curse-on-hit affix). A real curse's
    // full machinery is not modelled; each kind carries its headline effect, scaled by the curse level (level 1 is the
    // base value, +1 point per level, capped at the level-20 value).
    internal enum CurseKind { None, Vulnerability, TemporalChains, Enfeeble, Flammability, Frostbite, Conductivity }

    sealed class CurseState
    {
        public readonly List<(CurseKind Kind, double Until, int Level)> Active = [];
    }

    static readonly ConditionalWeakTable<ClientState, CurseState> Cursed = new();

    static void Prune(CurseState state, double now) => state.Active.RemoveAll(c => c.Until <= now);

    internal static bool CursedAt(ClientState conn, double now) => HasCurse(conn, now, null);

    internal static bool HasCurse(ClientState conn, double now, CurseKind? kind)
    {
        if (!Cursed.TryGetValue(conn, out var state)) return false;
        Prune(state, now);
        return state.Active.Any(c => kind is null || c.Kind == kind);
    }

    /// <summary>The active curse's level (0 when it is not on the player).</summary>
    internal static int CurseLevelAt(ClientState conn, double now, CurseKind kind)
    {
        if (!Cursed.TryGetValue(conn, out var state)) return 0;
        Prune(state, now);
        int at = state.Active.FindIndex(c => c.Kind == kind);
        return at >= 0 ? state.Active[at].Level : 0;
    }

    /// <summary>The level-scaled headline value: base at level 1, +<paramref name="perLevel"/> per level, capped.</summary>
    internal static int CurseValue(int level, int baseValue, int perLevel = 1, int cap = 0)
    {
        if (level <= 0) return 0;
        int value = baseValue + (Math.Min(level, 20) - 1) * perLevel;
        return cap > 0 ? Math.Min(value, cap) : value;
    }

    /// <summary>The headline value with curse_effect_on_self_+% applied (a curse on the player being stronger or
    /// weaker); 0 when the curse is not on the player.</summary>
    internal static int CursePoints(ClientState conn, double now, CurseKind kind, int baseValue, int perLevel = 1, int cap = 0)
    {
        int level = CurseLevelAt(conn, now, kind);
        if (level <= 0) return 0;
        int value = CurseValue(level, baseValue, perLevel, cap);
        int effect = Stats.Key("curse_effect_on_self_+%") is uint key && conn.PlayerStats.TryGetValue(key, out int pct) ? pct : 0;
        return (int)Math.Round(value * Math.Max(0, 100 + effect) / 100.0);
    }

    /// <summary>Add a curse (or refresh the same kind), respecting the allowed number of curses; the oldest goes when
    /// the cap is exceeded.</summary>
    internal static void SetCursed(ClientState conn, double until, CurseKind kind = CurseKind.None, int level = 1)
    {
        var state = Cursed.GetOrCreateValue(conn);
        double now = conn.Clock.Elapsed.TotalSeconds;
        Prune(state, now);
        int at = state.Active.FindIndex(c => c.Kind == kind);
        if (at >= 0) state.Active[at] = (kind, until, level);
        else state.Active.Add((kind, until, level));
        int extra = Stats.Key("number_of_additional_curses_allowed") is uint key && conn.PlayerStats.TryGetValue(key, out int value)
            ? Math.Max(0, value) : 0;
        // number_of_additional_curses_allowed_while_at_maximum_power_charges (a third slot at full charges).
        if (Stats.Key("number_of_additional_curses_allowed_while_at_maximum_power_charges") is uint maxKey
            && conn.PlayerStats.TryGetValue(maxKey, out int maxExtra) && maxExtra > 0)
        {
            lock (conn.Monsters)
            {
                var session = ChargesOf(conn);
                var limits = session.Limits(ChargeType.Power);
                if (limits.Maximum > 0 && session.Effective(ChargeType.Power) >= limits.Maximum) extra += maxExtra;
            }
        }
        int cap = 1 + Math.Max(0, extra);
        while (state.Active.Count > cap) state.Active.RemoveAt(0);
    }

    /// <summary>Clear every curse (dev: /curse with no argument).</summary>
    internal static void ClearCurses(ClientState conn) =>
        Cursed.GetOrCreateValue(conn).Active.Clear();
}

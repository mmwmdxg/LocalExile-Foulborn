using System.Runtime.CompilerServices;

static partial class Game
{
    // The cursed state the while_cursed modifiers read (dev: /curse, or a monster's curse-on-hit affix). A real curse's
    // full machinery is not modelled; the common kinds carry their headline effect only, and the number of curses that
    // can stack follows number_of_additional_curses_allowed (base one).
    internal enum CurseKind { None, Vulnerability, TemporalChains, Enfeeble }

    sealed class CurseState
    {
        public readonly List<(CurseKind Kind, double Until)> Active = [];
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

    /// <summary>Add a curse (or refresh the same kind), respecting the allowed number of curses; the oldest goes when
    /// the cap is exceeded.</summary>
    internal static void SetCursed(ClientState conn, double until, CurseKind kind = CurseKind.None)
    {
        var state = Cursed.GetOrCreateValue(conn);
        double now = conn.Clock.Elapsed.TotalSeconds;
        Prune(state, now);
        int at = state.Active.FindIndex(c => c.Kind == kind);
        if (at >= 0) state.Active[at] = (kind, until);
        else state.Active.Add((kind, until));
        int extra = Stats.Key("number_of_additional_curses_allowed") is uint key && conn.PlayerStats.TryGetValue(key, out int value)
            ? Math.Max(0, value) : 0;
        int cap = 1 + extra;
        while (state.Active.Count > cap) state.Active.RemoveAt(0);
    }

    /// <summary>Clear every curse (dev: /curse with no argument).</summary>
    internal static void ClearCurses(ClientState conn) =>
        Cursed.GetOrCreateValue(conn).Active.Clear();
}

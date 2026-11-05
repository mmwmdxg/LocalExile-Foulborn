using System.Runtime.CompilerServices;

static partial class Game
{
    // The cursed state the while_cursed modifiers read (dev: /curse, or a monster's curse-on-hit affix). A real curse's
    // full machinery is not modelled; the common kinds carry their headline effect only.
    internal enum CurseKind { None, Vulnerability, TemporalChains, Enfeeble }

    sealed class CurseState
    {
        public double Until;
        public CurseKind Kind;
    }

    static readonly ConditionalWeakTable<ClientState, CurseState> Cursed = new();

    internal static bool CursedAt(ClientState conn, double now) =>
        Cursed.TryGetValue(conn, out var state) && now < state.Until;

    internal static CurseKind CurseAt(ClientState conn, double now) =>
        Cursed.TryGetValue(conn, out var state) && now < state.Until ? state.Kind : CurseKind.None;

    internal static void SetCursed(ClientState conn, double until, CurseKind kind = CurseKind.None)
    {
        var state = Cursed.GetOrCreateValue(conn);
        state.Until = until;
        state.Kind = kind;
    }
}

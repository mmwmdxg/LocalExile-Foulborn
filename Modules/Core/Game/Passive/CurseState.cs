using System.Runtime.CompilerServices;

static partial class Game
{
    // The cursed state the while_cursed modifiers read (dev: /curse). A curse a monster casts is not modelled yet, so
    // the state is kept here per connection and set by the command.
    sealed class CurseState { public double Until; }

    static readonly ConditionalWeakTable<ClientState, CurseState> Cursed = new();

    internal static bool CursedAt(ClientState conn, double now) =>
        Cursed.TryGetValue(conn, out var state) && now < state.Until;

    internal static void SetCursed(ClientState conn, double until)
    {
        var state = Cursed.GetOrCreateValue(conn);
        state.Until = until;
    }
}

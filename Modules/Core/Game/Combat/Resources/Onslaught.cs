using System.Net.Sockets;
using System.Runtime.CompilerServices;
using GameServer.Core.Protocol.Instance;
using GameServer.Core.Protocol.Login;

static partial class Game
{
    // Onslaught: BuffDefinitions 215 is the client icon; the server applies the +20% action speed and publishes the
    // buff. Only the "while at maximum endurance charges" grant is modelled so far.
    sealed class OnslaughtState
    {
        public double Until;
        public ushort Buff;
        public bool Shown;
    }

    const ushort OnslaughtBuffRow = 215;
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

    /// <summary>Publish or withdraw the Onslaught icon on the player (player tick).</summary>
    internal static async Task SendOnslaught(string remote, NetworkStream stream, ConnectionCiphers ciphers, ClientState conn)
    {
        bool active = OnslaughtActive(conn, conn.Clock.Elapsed.TotalSeconds);
        var state = Onslaughts.GetOrCreateValue(conn);
        if (state.Shown == active) return;
        state.Shown = active;
        if (active)
        {
            state.Buff = conn.NextBuff++;
            await Send(remote, stream, ciphers, InstanceEntities.AddBuff(PlayerId, state.Buff, OnslaughtBuffRow, PlayerId),
                "0x01c1 player buff: Onslaught");
        }
        else
        {
            await Send(remote, stream, ciphers, InstanceEntities.RemoveBuff(PlayerId, state.Buff),
                "0x01c2 player buff: Onslaught");
        }
    }
}

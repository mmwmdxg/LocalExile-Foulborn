using System.Runtime.CompilerServices;

static partial class Game
{
    // Curses on monsters (a player's curse skill, or the dev command). Each monster keeps its own list; the damage
    // taken they add is folded into SpawnedMonster.TakenFromPlayer where that is rebuilt each tick.
    sealed class MonsterCurseEntry
    {
        public CurseKind Kind;
        public double Until;
        public int Level;
    }

    static readonly ConditionalWeakTable<ClientState, Dictionary<uint, List<MonsterCurseEntry>>> MonsterCurses = new();

    // The active skill's effect id when it is a curse skill.
    internal static CurseKind CurseSkillKind(string? effect) => effect switch
    {
        "vulnerability" => CurseKind.Vulnerability,
        "temporal_chains" => CurseKind.TemporalChains,
        "enfeeble" => CurseKind.Enfeeble,
        "flammability" => CurseKind.Flammability,
        "frostbite" => CurseKind.Frostbite,
        "conductivity" => CurseKind.Conductivity,
        _ => CurseKind.None,
    };

    internal static bool MonsterHexproof(SpawnedMonster m) =>
        m.Mods.Effects["base_immune_to_curses"] != 0 || m.Mods.Effects["curse_immune"] != 0;

    /// <summary>Put a curse on one monster (hexproof monsters refuse it). Returns false when refused.</summary>
    internal static bool SetMonsterCurse(ClientState conn, uint id, SpawnedMonster m, CurseKind kind, double seconds, int level = 1)
    {
        if (kind == CurseKind.None || MonsterHexproof(m)) return false;
        var map = MonsterCurses.GetOrCreateValue(conn);
        if (!map.TryGetValue(id, out var list)) map[id] = list = [];
        list.RemoveAll(e => e.Kind == kind);
        list.Add(new MonsterCurseEntry { Kind = kind, Until = conn.Clock.Elapsed.TotalSeconds + seconds, Level = level });
        return true;
    }

    internal static void ClearMonsterCurses(ClientState conn, uint id) =>
        MonsterCurses.GetOrCreateValue(conn).Remove(id);

    /// <summary>What the monster's curses add to the damage it takes: Vulnerability to every type, the elemental
    /// curses to elemental damage.</summary>
    internal static (int All, int Elemental) MonsterCurseDamageTaken(ClientState conn, uint id, double now)
    {
        if (!MonsterCurses.TryGetValue(conn, out var map) || !map.TryGetValue(id, out var list)) return (0, 0);
        list.RemoveAll(e => e.Until <= now);
        int all = 0, elemental = 0;
        foreach (var entry in list)
        {
            int value = CurseValue(entry.Level, 20, 1, 40);
            if (entry.Kind == CurseKind.Vulnerability) all += value;
            else if (entry.Kind is CurseKind.Flammability or CurseKind.Frostbite or CurseKind.Conductivity) elemental += value;
        }
        return (all, elemental);
    }
}

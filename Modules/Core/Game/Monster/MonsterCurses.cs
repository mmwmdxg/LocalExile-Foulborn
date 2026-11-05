using System.Runtime.CompilerServices;

static partial class Game
{
    // Curses on monsters (a player's curse skill or mark, or the dev command). Keyed by the monster object, so callers
    // do not need its id; the damage taken they add is folded into SpawnedMonster.TakenFromPlayer where that is rebuilt.
    sealed class MonsterCurseEntry
    {
        public CurseKind Kind;
        public double Until;
        public int Level;
    }

    static readonly ConditionalWeakTable<SpawnedMonster, List<MonsterCurseEntry>> MonsterCurses = new();

    internal static bool MonsterHexproof(SpawnedMonster m) =>
        m.Mods.Effects["base_immune_to_curses"] != 0 || m.Mods.Effects["curse_immune"] != 0;

    // The active skill's effect id when it is a curse or mark skill.
    internal static CurseKind CurseSkillKind(string? effect) => effect switch
    {
        "vulnerability" => CurseKind.Vulnerability,
        "temporal_chains" => CurseKind.TemporalChains,
        "enfeeble" => CurseKind.Enfeeble,
        "flammability" => CurseKind.Flammability,
        "frostbite" => CurseKind.Frostbite,
        "conductivity" => CurseKind.Conductivity,
        "assassins_mark" or "assassins_mark_triggered" => CurseKind.AssassinsMark,
        "poachers_mark" or "poachers_mark_triggered" => CurseKind.PoachersMark,
        "warlords_mark" or "warlords_mark_triggered" => CurseKind.WarlordsMark,
        "snipers_mark" => CurseKind.SnipersMark,
        _ => CurseKind.None,
    };

    /// <summary>Put a curse on one monster (hexproof monsters refuse it). Returns false when refused.</summary>
    internal static bool SetMonsterCurse(ClientState conn, SpawnedMonster m, CurseKind kind, double seconds, int level = 1)
    {
        if (kind == CurseKind.None || MonsterHexproof(m)) return false;
        var list = MonsterCurses.GetOrCreateValue(m);
        list.RemoveAll(e => e.Kind == kind);
        list.Add(new MonsterCurseEntry { Kind = kind, Until = conn.Clock.Elapsed.TotalSeconds + seconds, Level = level });
        return true;
    }

    internal static bool MonsterHasCurse(ClientState conn, SpawnedMonster m, double now, CurseKind kind)
    {
        if (!MonsterCurses.TryGetValue(m, out var list)) return false;
        list.RemoveAll(e => e.Until <= now);
        return list.Any(e => e.Kind == kind);
    }

    /// <summary>What the monster's curses add to the damage it takes: Vulnerability and Assassin's Mark to every type,
    /// the elemental curses to elemental damage.</summary>
    internal static (int All, int Elemental) MonsterCurseDamageTaken(ClientState conn, SpawnedMonster m, double now)
    {
        if (!MonsterCurses.TryGetValue(m, out var list)) return (0, 0);
        list.RemoveAll(e => e.Until <= now);
        int all = 0, elemental = 0;
        foreach (var entry in list)
        {
            int value = CurseValue(entry.Level, 20, 1, 40);
            if (entry.Kind is CurseKind.Vulnerability or CurseKind.AssassinsMark) all += value;
            else if (entry.Kind is CurseKind.Flammability or CurseKind.Frostbite or CurseKind.Conductivity) elemental += value;
        }
        return (all, elemental);
    }
}

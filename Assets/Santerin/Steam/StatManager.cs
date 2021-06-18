using Steamworks;

/// <summary>
/// Manage current user's stats.
/// </summary>
public static class StatManager
{
    public static void Set(Stat stat, int value)
    {
        if (!StatsBase.StatsInitialized) return;
        SteamUserStats.SetStat(stat.ToString(), value);
        StatsBase.SaveAll();
    }

    public static void Update(Stat stat, int value)
    {
        if (!StatsBase.StatsInitialized) return;
        SteamUserStats.SetStat(stat.ToString(), Get(stat) + value);
        StatsBase.SaveAll();
    }

    public static int Get(Stat stat)
    {
        if (!StatsBase.StatsInitialized) return 0;
        SteamUserStats.GetStat(stat.ToString(), out int value);
        return value;
    }
}
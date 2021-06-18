using Steamworks;
using System;

public static class StatsBase
{
    public static event EventHandler OnStatsInitializedEvent;

    public static Callback<UserStatsStored_t> UserStatsStoredCallback { get; } = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
    private static void OnUserStatsStored(UserStatsStored_t data)
    {
        OnUserStatsStoredEvent.Invoke(null, EventArgs.Empty);
    }

    public static event EventHandler OnUserStatsStoredEvent;

    public static bool StatsInitialized
    {
        get => SteamManager.Instance.Initialized && HasCurrentStats;
    }

    public static bool HasCurrentStats { get; private set; }

    public static void InitializeStatsBase()
    {
        HasCurrentStats = SteamUserStats.RequestCurrentStats();
        if (StatsInitialized)
        {
            OnStatsInitializedEvent?.Invoke(null, EventArgs.Empty);
        }
    }

    public static void SaveAll()
    {
        if (!StatsInitialized) return;
        SteamUserStats.StoreStats();
    }

    public static void ResetAll()
    {
        if (!StatsInitialized) return;
        SteamUserStats.ResetAllStats(bAchievementsToo: true);
    }
}
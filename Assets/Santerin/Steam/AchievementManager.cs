using Steamworks;

/// <summary>
/// Manage current user's achievements.
/// </summary>
public static class AchievementManager
{
    /// <summary>
    /// Checks if an achievement is activated, if it isnt, then activate it.
    /// </summary>
    /// <param name="achievement"></param>
    public static void Activate(Achievement achievement)
    {
        if (!StatsBase.StatsInitialized) return;
        if (!IsAchieved(achievement).GetValueOrDefault())
        {
            SteamUserStats.SetAchievement(achievement.APIName);
            StatsBase.SaveAll();
        }
    }

    public static bool? IsAchieved(Achievement achievement)
    {
        if (!StatsBase.StatsInitialized) return null;
        SteamUserStats.GetAchievement(achievement.APIName, out bool achieved);
        return achieved;
    }
}
using Steamworks;

public class RichPresenceManager : Singleton<RichPresenceManager>
{
    private void OnEnable()
    {
        StatsBase.OnUserStatsStoredEvent += OnUserStatsStored;
    }

    private void OnUserStatsStored(object sender, System.EventArgs e)
    {
        UpdateRichPresence();
    }

    private static void UpdateRichPresence()
    {
        if (SteamManager.Instance.Initialized)
        {
            SteamFriends.SetRichPresence("steam_display", $"{StatManager.Get(Stat.ST_SKELETON_KILLS)} skeletons slain");
        }
    }

    private void OnDisable()
    {
        StatsBase.OnUserStatsStoredEvent -= OnUserStatsStored;
    }
}
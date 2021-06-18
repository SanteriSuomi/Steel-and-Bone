using Steamworks;
using System;

public static class SteamCallbacks
{
    public static Callback<GameOverlayActivated_t> OverlayActivatedCallback { get; private set; }
    public static event EventHandler OnOverlayActivatedEvent;

    public static void InitializeSteamCallbacks()
    {
        OverlayActivatedCallback = Callback<GameOverlayActivated_t>.Create(OverlayActivated);
    }

    private static void OverlayActivated(GameOverlayActivated_t callback)
    {
        OnOverlayActivatedEvent?.Invoke(null, EventArgs.Empty);
    }
}
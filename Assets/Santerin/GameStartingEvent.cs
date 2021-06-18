using UnityEngine;

public class GameStartingEvent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TriggerEnterEvent();
        }
    }

    private static void TriggerEnterEvent()
    {
        AchievementManager.Activate(Achievements.Get("GAME_START"));
    }
}
using UnityEngine;

public class PuzzleAchievementTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            AchievementManager.Activate(Achievements.Get("PUZZLE_COMPLETED"));
        }
    }
}
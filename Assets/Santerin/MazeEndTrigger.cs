using UnityEngine;

public class MazeEndTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider _)
    {
        QuestContext solveMazeQuest = QuestManager.Instance.GetQuest(2);
        solveMazeQuest.Data.IsCompleted = true;
    }
}
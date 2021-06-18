using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndingEvent : MonoBehaviour
{
    [SerializeField]
    private int outroSceneIndex = 3;

	private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TriggerEnterEvent();
        }
    }

    private void TriggerEnterEvent()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AchievementManager.Activate(Achievements.Get("GAME_COMPLETE"));
        SceneManager.LoadScene(outroSceneIndex, LoadSceneMode.Single);
    }
}
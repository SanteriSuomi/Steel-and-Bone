using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndGameScreenManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI quitText = default;

    private void Start()
    {
        Invoke(nameof(EndGame), 30);
        InvokeRepeating(nameof(FlashQuitText), 0, 0.9f); // Flash the quit text button
    }

    private void Update()
    {
        if ((Keyboard.current?.escapeKey.wasPressedThisFrame == true)
			|| (Gamepad.current?.startButton.wasPressedThisFrame == true))
        {
            EndGame();
        }
    }

    private void FlashQuitText()
    {
        quitText.gameObject.SetActive(!quitText.gameObject.activeSelf);
    }

    private void EndGame()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
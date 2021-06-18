using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
	private void Start()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	private void Update()
	{
        if ((Keyboard.current?.escapeKey.wasPressedThisFrame == true)
			|| (Gamepad.current?.startButton.wasPressedThisFrame == true))
        {
            MainMenu();
        }
	}

	public void MainMenu()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(0, LoadSceneMode.Single);
	}

	public void QuitGame()
	{
		Process.GetCurrentProcess().Kill();
	}

	public void Restart()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(2, LoadSceneMode.Single);
	}
}
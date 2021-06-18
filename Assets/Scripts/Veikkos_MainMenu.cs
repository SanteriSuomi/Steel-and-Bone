using UnityEngine;
using UnityEngine.SceneManagement;

public class Veikkos_MainMenu : MonoBehaviour
{
	public void QuitGame()
	{
		Application.Quit();
	}

	public void Options()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(3);
	}

	public void Restart()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(1);
	}
}
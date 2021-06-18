using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
	public AudioClip mainTheme;
	public AudioClip menuTheme;
	string sceneName;

	void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoad;
	}

	private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
	{
		string newSceneName = SceneManager.GetActiveScene().name;
		if (newSceneName != sceneName)
		{
			sceneName = newSceneName;
			Invoke(nameof(PlayMusic), .2f);
		}
	}

	void PlayMusic()
	{
		AudioClip clipToPlay = null;

		if (sceneName == "Menu")
		{
			clipToPlay = menuTheme;
		}
		else if (sceneName == "Game")
		{
			clipToPlay = mainTheme;
		}

		if (clipToPlay != null)
		{
			AudioManager.Instance.PlayMusic(clipToPlay, 2);
			Invoke("PlayMusic", clipToPlay.length);
		}
	}
}

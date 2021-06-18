using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
	[SerializeField]
	private VideoPlayer introVideoPlayer = default;
	[SerializeField]
	private GameObject m_EscSkip = default;

	private void Awake()
	{
		introVideoPlayer.loopPointReached += OnVideoEnded;
		InvokeRepeating(nameof(FlashESCText), 0, 0.9f);
	}

	private void OnVideoEnded(VideoPlayer source)
	{
		SceneManager.LoadScene(2, LoadSceneMode.Single);
	}

	private void Update()
	{
		if ((Keyboard.current?.escapeKey.wasPressedThisFrame == true) 
			|| (Gamepad.current?.startButton.wasPressedThisFrame == true))
		{
			SceneManager.LoadScene(2, LoadSceneMode.Single);
		}
	}

	private void FlashESCText()
	{
		m_EscSkip.SetActive(!m_EscSkip.activeSelf);
	}
}
using Essentials.Saving;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
//using UnityEngine.UI;

public class Veikkos_MenuManager : MonoBehaviour
{
	[SerializeField] private GameObject m_pauseMenuUI = default;
	[SerializeField] private Aleksi_CursorHide cursorHide = default;
	[SerializeField]
	private GameObject classPickerMenu = default;
	[SerializeField]
	private GameObject abilityPickerMenu = default;
	[SerializeField]
	private GameObject menuSaveButton = default;

	private bool m_GameIsPaused;

	private void Awake()
	{
		//pauseMenuScaler = m_pauseMenuUI.GetComponent<CanvasScaler>();
		SteamCallbacks.OnOverlayActivatedEvent += OnOverlayActivated;
	}

	private void OnOverlayActivated(object sender, EventArgs e)
	{
		m_GameIsPaused = false;
		DeterminePauseState();
	}

	private void Update()
	{
		if (classPickerMenu.activeSelf
			|| abilityPickerMenu.activeSelf)
		{
			return;
		}

		if (Gamepad.current != null)
		{
			if (menuSaveButton.gameObject.activeSelf)
			{
				menuSaveButton.gameObject.SetActive(false);
			}
		}
		else if (!menuSaveButton.gameObject.activeSelf)
		{
			menuSaveButton.gameObject.SetActive(true);
		}

		if ((Keyboard.current?.escapeKey.wasPressedThisFrame == true)
			|| (Gamepad.current?.startButton.wasPressedThisFrame == true))
		{
			if (Gamepad.current != null)
			{
				// Automatically save when gamepad connected.
				SaveManager.Instance.Save();
			}

			DeterminePauseState();
		}
	}

	private void DeterminePauseState()
	{
		//if (!canSwitch) return;
		//canSwitch = false;

		if (m_GameIsPaused)
		{
			cursorHide.enabled = true;
			Resume();
		}
		else
		{
			cursorHide.enabled = false;
			Pause();
		}
	}

	public void Resume()
	{
		Time.timeScale = 1f;
		AudioListener.volume = 1;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		m_GameIsPaused = false;
		m_pauseMenuUI.SetActive(false);

		#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		SBUtils.MouseClick(MouseClickEvents.LEFTDOWN, new Vector2(Screen.width / 2, Screen.height / 2));
		#endif

		//pauseMenuScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
		//CalculateNormalizedHeightRes();
		//Time.timeScale = 1f;
		//AudioListener.volume = 1;
		//Cursor.visible = false;
		//Cursor.lockState = CursorLockMode.Locked;

		//var tween = DOTween.To(() => pauseMenuScaler.scaleFactor, x => pauseMenuScaler.scaleFactor = x, 0, menuAnimDuration)
		//		.SetRecyclable(true)
		//		.SetEase(Ease.OutExpo)
		//		.SetUpdate(isIndependentUpdate: true);

		//tween.OnUpdate(() =>
		//{
		//	if (tween.ElapsedPercentage() >= 0.9)
		//	{
		//		m_GameIsPaused = false;
		//		canSwitch = true;
		//		m_pauseMenuUI.SetActive(false);
		//		#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		//		LeftClickMouse();
		//		#endif
		//	}

		//}).OnComplete(() =>
		//{
		//	ForceUpdateMenu();
		//});
	}

	private void Pause()
	{
		m_pauseMenuUI.SetActive(true);
		AudioListener.volume = 0;
		Time.timeScale = 0f;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		m_GameIsPaused = true;

		//pauseMenuScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
		//CalculateNormalizedHeightRes();
		//m_pauseMenuUI.SetActive(true);

		//var tween = DOTween.To(() => pauseMenuScaler.scaleFactor, x => pauseMenuScaler.scaleFactor = x, normalizedHeightRes, menuAnimDuration)
		//		.SetRecyclable(true)
		//		.SetEase(Ease.OutExpo)
		//		.SetUpdate(isIndependentUpdate: true);

		//tween.OnUpdate(() =>
		//{
		//	if (tween.ElapsedPercentage() >= 0.9f)
		//	{
		//		AudioListener.volume = 0;
		//		Time.timeScale = 0f;
		//		Cursor.visible = true;
		//		Cursor.lockState = CursorLockMode.None;
		//		m_GameIsPaused = true;
		//		canSwitch = true;
		//	}

		//}).OnComplete(() =>
		//{
		//	ForceUpdateMenu();
		//});
	}

	//private void ForceUpdateMenu()
	//{
	//	pauseMenuScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
	//	LayoutRebuilder.ForceRebuildLayoutImmediate(pauseMenuScaler.transform as RectTransform);
	//}

	//private void CalculateNormalizedHeightRes()
	//{
	//	float averageDivisor;
	//	if (Screen.currentResolution.height < 1440)
	//	{
	//		averageDivisor = 3;
	//	}
	//	else
	//	{
	//		averageDivisor = 2;
	//	}

	//	normalizedHeightRes = (Screen.currentResolution.height + Screen.currentResolution.width) / averageDivisor / 1000;
	//}

	#pragma warning disable CA1822 // Cannot make methods static as they're referenced by unity events
	public void MainMenu()
	{
		AudioListener.volume = 1;
		Time.timeScale = 1f;
		SceneManager.LoadScene(0);
	}

	public void QuitGame()
	{
		Process.GetCurrentProcess().Kill();
		//Application.Quit();
	}

	public void Options()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene(3);
	}

	public void Restart()
	{
		AudioListener.volume = 1;
		Time.timeScale = 1;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
	}
}
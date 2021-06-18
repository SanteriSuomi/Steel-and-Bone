using Steamworks;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField]
	private GameObject dropDownMenuObj = default;
	[SerializeField]
	private GameDropdownController dropDownController = default;
	private TMP_Dropdown dropDownMenu;
	private string defaultDropdownLabel;
	[SerializeField]
	private RawImage saveImage = default;
	public static bool SaveScreenshotExists { get; private set; }

	private void Awake()
	{
		dropDownMenu = dropDownMenuObj.GetComponent<TMP_Dropdown>();
		defaultDropdownLabel = dropDownMenu.captionText.text;
		dropDownMenu.onValueChanged.AddListener(PlayDropDownEvent);
	}

	private void Start()
	{
		SaveScreenshotExists = SteamRemoteStorage.FileExists("savescreenshot.dat");
		if (SteamManager.Instance.Initialized
			&& SaveScreenshotExists)
		{
			int fileSize = SteamRemoteStorage.GetFileSize("savescreenshot.dat");
			byte[] screenshotData = new byte[fileSize];
			SteamRemoteStorage.FileRead("savescreenshot.dat", screenshotData, fileSize);

			Texture2D newImage = new Texture2D((int)saveImage.rectTransform.sizeDelta.x, (int)saveImage.rectTransform.sizeDelta.y, TextureFormat.RGB24, false);
			newImage.LoadImage(screenshotData);
			newImage.Apply();
			saveImage.texture = newImage;
		}
	}

	private void PlayDropDownEvent(int dropValue)
	{
		if (dropValue == 1)
		{
			LoadNotifier.CanLoad = true;
			Play(2);
		}
		else if (dropValue == 2)
		{
			Play(1);
		}
	}

	public void QuitGame()
	{
		Process.GetCurrentProcess().Kill();
	}

	public void Options()
	{
		Time.timeScale = 1f;
	}

	public void PlayGamePopup()
	{
		if (!dropDownController.gameObject.activeSelf)
		{
			dropDownMenuObj.SetActive(true);
		}
	}

	private void Play(int sceneIndex)
	{
		Time.timeScale = 1f;
		dropDownMenu.captionText.text = defaultDropdownLabel;
		SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
	}
}
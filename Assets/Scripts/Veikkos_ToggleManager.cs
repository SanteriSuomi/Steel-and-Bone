using UnityEngine;
using UnityEngine.UI;

public class Veikkos_ToggleManager : MonoBehaviour
{
	

	[SerializeField] public Toggle[] m_ResolutionToggles;
	[SerializeField] public Toggle m_FullscreenToggle;
	[SerializeField] public int[] m_ScreenWidths;
	[SerializeField] int m_ActiveScreenResIndex;

	void Start()
	{
		

		m_ActiveScreenResIndex = PlayerPrefs.GetInt("screen res index");
		bool isFullscreen = (PlayerPrefs.GetInt("fullscreen") == 1) ? true : false;
		
		
		m_ActiveScreenResIndex = 5;
		

		for (int i = 0; i < m_ResolutionToggles.Length; ++i)
		{
			m_ResolutionToggles[i].isOn = i == m_ActiveScreenResIndex;
		}

		m_FullscreenToggle.isOn = true;
		SetFullscreen(isFullscreen);

		
	}

	public void SetScreenResolution(int i)
	{
		if (m_ResolutionToggles[i].isOn)
		{
			m_ActiveScreenResIndex = i;
			float aspectRatio = 16 / 9f;
			Screen.SetResolution(m_ScreenWidths[i], (int)(m_ScreenWidths[i] / aspectRatio), true);  
			
			PlayerPrefs.SetInt("screen res index", m_ActiveScreenResIndex);
			PlayerPrefs.Save();
		}
		if (m_ResolutionToggles[i].isOn && m_FullscreenToggle.isOn == false)
		{
			m_ActiveScreenResIndex = i;
			float aspectRatio = 16 / 9f;
			Screen.SetResolution(m_ScreenWidths[i], (int)(m_ScreenWidths[i] / aspectRatio), false);  

			PlayerPrefs.SetInt("screen res index", m_ActiveScreenResIndex);
			PlayerPrefs.Save();
		}
	}

	

	public void SetFullscreen(bool isFullscreen)
	{
		for (int i = 0; i < m_ResolutionToggles.Length; i++)
		{
		//	m_ResolutionToggles[i].interactable = !isFullscreen;
		}
		if (isFullscreen)
		{
			Resolution[] allResolutions = Screen.resolutions;
			Resolution maxResolution = allResolutions[allResolutions.Length - 1];
			Screen.SetResolution(maxResolution.width, maxResolution.height, true); 
			m_FullscreenToggle.isOn = true;
		}
		else
		{
			
			SetScreenResolutionWindowed(m_ActiveScreenResIndex);
			m_FullscreenToggle.isOn = false;


		}

		PlayerPrefs.SetInt("fullscreen", ((isFullscreen) ? 1 : 0));
		PlayerPrefs.Save();
	}
	public void SetScreenResolutionWindowed(int i) //sets fullscreen and resolutions toggle react with fullscreen toggle(u can change screen to windowed)
	{
		if (m_ResolutionToggles[i].isOn)
		{
			
			m_ActiveScreenResIndex = i;
			float aspectRatio = 16 / 9f;
			Screen.SetResolution(m_ScreenWidths[i], (int)(m_ScreenWidths[i] / aspectRatio), false);

			
			
			


			PlayerPrefs.SetInt("screen res index", m_ActiveScreenResIndex);
			PlayerPrefs.Save();
		}

	}
}
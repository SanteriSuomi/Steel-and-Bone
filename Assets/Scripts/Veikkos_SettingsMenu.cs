using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Veikkos_SettingsMenu : MonoBehaviour
{
	public AudioMixer m_FxMixer;
	public AudioMixer m_MusicVolume;
	public Slider fxVolumeSlider;
	[SerializeField]
	private Slider musicVolumeSlider = default;
	public Dropdown m_Quality;
	public Slider m_BrightnessSlider;

	private void Start()
	{
		try
		{
			m_Quality.value = PlayerPrefs.GetInt("Quality");
			QualitySettings.SetQualityLevel(3);
			m_Quality.value = 3;
			fxVolumeSlider.value = PlayerPrefs.GetFloat("FxVolume");
			musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
			m_FxMixer.SetFloat("volume", fxVolumeSlider.value);
			m_MusicVolume.SetFloat("volume", musicVolumeSlider.value);
			m_BrightnessSlider.value = PlayerPrefs.GetFloat("Brightness");
			m_BrightnessSlider.onValueChanged.AddListener(SetLightIntensityFromMenu);
		}
		catch (System.NullReferenceException)
		{
			// Intentionally left empty.
		}
	}

	public void SetQuality(int qualityIndex)
	{
		if (m_Quality == null) return;
		QualitySettings.SetQualityLevel(qualityIndex);
		SaveSettingTo("Quality", m_Quality.value);
	}

	public void SetVolume(float volume)
	{
		if (volume <= 0.1f)
		{
			m_FxMixer.SetFloat("volume", -100);
		}
		else
		{
			m_FxMixer.SetFloat("volume", volume);
		}

		SaveSettingTo("FxVolume", fxVolumeSlider.value);
	}

	public void SetMusicVolume(float volume)
	{
		if (volume <= 0.1f)
		{
			m_MusicVolume.SetFloat("volume", -100);
		}
		else
		{
			m_MusicVolume.SetFloat("volume", volume);
		}

		SaveSettingTo("MusicVolume", musicVolumeSlider.value);
	}

	private void SetLightIntensityFromMenu(float value)
	{
		SaveSettingTo("Brightness", value);
	}

	private void SaveSettingTo(string file, float value)
	{
		PlayerPrefs.SetFloat(file, value);
		PlayerPrefs.Save();
	}
}
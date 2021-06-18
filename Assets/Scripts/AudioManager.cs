using System.Collections;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	public enum AudioChannel { Master, Sfx, Music };
	public float MasterVolumePercent { get; private set; }
	public float SFXVolumePercent { get; private set; }
	public float MusicVolumePercent { get; private set; }

	AudioSource[] musicSources;
	int activeMusicSourceIndex;

	protected override void Awake()
	{
		musicSources = new AudioSource[2];
		for (int i = 0; i < 2; i++)
		{
			GameObject newMusicSource = new GameObject("Music source " + (i + 1));
			musicSources[i] = newMusicSource.AddComponent<AudioSource>();
			newMusicSource.transform.parent = transform;
		}
		GameObject newSfx2Dsource = new GameObject("2D sfx source");
		newSfx2Dsource.transform.parent = transform;

		MasterVolumePercent = PlayerPrefs.GetFloat("master vol", 1);
		SFXVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1);
		MusicVolumePercent = PlayerPrefs.GetFloat("music vol", 1);
	}

	public void SetVolume(float volumePercent, AudioChannel channel)
	{
		switch (channel)
		{
			case AudioChannel.Master:
				MasterVolumePercent = volumePercent;
				break;
			case AudioChannel.Sfx:
				SFXVolumePercent = volumePercent;
				break;
			case AudioChannel.Music:
				MusicVolumePercent = volumePercent;
				break;
		}

		musicSources[0].volume = MusicVolumePercent * MasterVolumePercent;
		musicSources[1].volume = MusicVolumePercent * MasterVolumePercent;

		PlayerPrefs.SetFloat("master vol", MasterVolumePercent);
		PlayerPrefs.SetFloat("sfx vol", SFXVolumePercent);
		PlayerPrefs.SetFloat("music vol", MusicVolumePercent);
		PlayerPrefs.Save();
	}

	public void PlayMusic(AudioClip clip, float fadeDuration = 1)
	{
		activeMusicSourceIndex = 1 - activeMusicSourceIndex;
		musicSources[activeMusicSourceIndex].clip = clip;
		musicSources[activeMusicSourceIndex].Play();

		StartCoroutine(AnimateMusicCrossfade(fadeDuration));
	}

	public void PlaySound(AudioClip clip, Vector3 pos)
	{
		if (clip != null)
		{
			AudioSource.PlayClipAtPoint(clip, pos, SFXVolumePercent * MasterVolumePercent);
		}
	}

	IEnumerator AnimateMusicCrossfade(float duration)
	{
		float percent = 0;

		while (percent < 1)
		{
			percent += Time.deltaTime * 1 / duration;
			musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, MusicVolumePercent * MasterVolumePercent, percent);
			musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(MusicVolumePercent * MasterVolumePercent, 0, percent);
			yield return null;
		}
	}
}
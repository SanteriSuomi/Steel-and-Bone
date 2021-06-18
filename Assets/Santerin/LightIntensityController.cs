using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LightIntensityController : MonoBehaviour
{
	[SerializeField]
	private Slider brightnessSlider = default;

	private (Light light, (float range, float intensity) lightInfo)[] lightData;
	private Coroutine applyLightCoroutine;

	private void Awake()
	{
		Light[] lights = FindObjectsOfType<Light>();
		float[] lightRanges = new float[lights.Length];
		float[] lightIntensities = new float[lights.Length];
		for (int i = 0; i < lights.Length; i++)
		{
			lightRanges[i] = lights[i].range;
			lightIntensities[i] = lights[i].intensity;
		}

		lightData = new (Light, (float, float))[lights.Length];
		for (int i = 0; i < lightData.Length; i++)
		{
			if (lights[i].gameObject.name != "Lantern")
			{
				lightData[i] = (lights[i], (lightRanges[i], lightIntensities[i]));
			}
		}
	}

	private void Start()
	{
		brightnessSlider.value = PlayerPrefs.GetFloat("Brightness");
		ApplyLightIntensity(brightnessSlider.value);
	}

	public void ApplyLightIntensity(float value)
	{
		if (applyLightCoroutine != null)
		{
			StopCoroutine(applyLightCoroutine);
		}

		applyLightCoroutine = StartCoroutine(ApplyLightIntensityCoroutine(value));
	}

	private IEnumerator ApplyLightIntensityCoroutine(float value)
	{
		if (lightData is null) yield break;

		for (int i = 0; i < lightData.Length; i++)
		{
			if (lightData[i].light != null)
			{
				lightData[i].light.range = lightData[i].lightInfo.range * value / 5;
				lightData[i].light.intensity = lightData[i].lightInfo.intensity * (value / 5);
				if (i == lightData.Length / 2)
				{
					yield return null; // Split work done to two frames.
				}
			}
		}
	}
}
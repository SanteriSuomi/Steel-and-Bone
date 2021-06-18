using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BrightnessCallback : MonoBehaviour, IPointerUpHandler
{
    [SerializeField]
    private LightIntensityController intensityController = default;
    [SerializeField]
    private Slider brightnessSlider = default;

    public void OnPointerUp(PointerEventData eventData)
    {
        intensityController.ApplyLightIntensity(brightnessSlider.value);
        PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);
        PlayerPrefs.Save();
    }
}
using UnityEngine;
using UnityEngine.UI;

public class Veikkos_MouseSensitivity : MonoBehaviour
{
    [SerializeField] public Slider m_MouseSlider = default;
    [SerializeField] private PlayerLook m_MouseLook = default;  //script have to be public to find it

    private void Awake()
    {
        m_MouseSlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1050);
        m_MouseLook.m_MouseSensitivity = m_MouseSlider.value / 3;
    }

    public void SetSensitivity(float sensitivity)
    {
        m_MouseLook.m_MouseSensitivity = m_MouseSlider.value / 3;
        SaveSensitivity();
    }

    private void OnDisable()
    {
        SaveSensitivity();
    }

    private void SaveSensitivity()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", m_MouseSlider.value);
        PlayerPrefs.Save();
    }
}
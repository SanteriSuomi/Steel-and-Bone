using UnityEngine;
using UnityEngine.UI;

public class Veikkos_VsyncDropdown : MonoBehaviour
{
    [SerializeField] public Dropdown m_VsyncDropDown;
    [SerializeField] public Dropdown m_AntiA;
    void Start()
    {
        QualitySettings.antiAliasing = 1;
        m_AntiA.value = 1;
        m_AntiA.value = PlayerPrefs.GetInt("Anti");
        PlayerPrefs.Save();


        QualitySettings.vSyncCount = 1;
        m_VsyncDropDown.value = 1;
        m_VsyncDropDown.value = PlayerPrefs.GetInt("Dropdown");
        PlayerPrefs.Save();
    }

    public void VsyncDropDown()
	{
        QualitySettings.vSyncCount = m_VsyncDropDown.value;
        PlayerPrefs.SetInt("Quality", m_VsyncDropDown.value);
        PlayerPrefs.Save();
    }
    public void AntiAliasing()
	{
       QualitySettings.antiAliasing = m_AntiA.value;
       PlayerPrefs.SetInt("Anti", m_AntiA.value);
       PlayerPrefs.Save();

    }



}
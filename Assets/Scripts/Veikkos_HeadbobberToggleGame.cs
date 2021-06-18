using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Veikkos_HeadbobberToggleGame : MonoBehaviour
{
	[SerializeField] public Veikkos_Headbobber m_Headbobber;
	[SerializeField] public Toggle m_HeadbobberToggle;
	//[SerializeField] private bool m_HasBeenToggled = true;

	void Awake()
	{
		//if (!PlayerPrefs.HasKey("Headbobber"))
		//{
		//	m_Headbobber.GetComponent<Veikkos_Headbobber>().enabled = true;
		//	m_HeadbobberToggle.isOn = true;
		//	PlayerPrefs.SetInt("Headbobber", 1);
		//	PlayerPrefs.Save();
		//}
	}

	void Start()
	{
		m_Headbobber.GetComponent<Veikkos_Headbobber>().enabled = true;
		m_HeadbobberToggle.isOn = true;
		//bool isHeadbobberOn = (PlayerPrefs.GetInt("Headbobber") == 1) ? true : false;
		//m_Headbobber.GetComponent<Veikkos_Headbobber>().enabled = isHeadbobberOn;
		//m_HeadbobberToggle.isOn = isHeadbobberOn;
		//PlayerPrefs.Save();
	}

	public void Headbobbing()
	{
		//switch (m_HasBeenToggled)
		//{
		//	case true:
		//		m_HasBeenToggled = false;
		//		m_Headbobber.GetComponent<Veikkos_Headbobber>().enabled = false;
		//		m_HeadbobberToggle.isOn = false;
		//	/Save System for bool
		//		PlayerPrefs.SetInt("Headbobber", 0);
		//		PlayerPrefs.Save();
		//		print("Headbobbing OFF");
		//		break;
		//	case false:
		//		m_HasBeenToggled = true;
		//		m_Headbobber.GetComponent<Veikkos_Headbobber>().enabled = true;
		//		m_HeadbobberToggle.isOn = true;
		//		PlayerPrefs.SetInt("Headbobber", 1);
		//		PlayerPrefs.Save();
		//		print("Headbobbing ON");
		//		break;
		//}

		if (!m_Headbobber.enabled)
		{
			m_Headbobber.enabled = true;
			m_HeadbobberToggle.isOn = true;
			//PlayerPrefs.SetInt("Headbobber", 0);
			//PlayerPrefs.Save();
		}
		else
		{
			m_Headbobber.enabled = false;
			m_HeadbobberToggle.isOn = false;
			//Save System for bool
			//PlayerPrefs.SetInt("Headbobber", 1);
			//PlayerPrefs.Save();
		}
	}
}
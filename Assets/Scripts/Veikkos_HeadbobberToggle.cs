using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Veikkos_HeadbobberToggle : MonoBehaviour
{
	[SerializeField] public GameObject m_Headbobber;
	[SerializeField] public Toggle m_HeadbobberToggle;
	[SerializeField] private bool m_HasBeenToggled = false;


	//void Awake()
	//{
	//	if (!PlayerPrefs.HasKey("Headbobber"))
	//	{

	//		m_Headbobber.GetComponent<Veikkos_Headbobber>().enabled = true;
	//		m_HeadbobberToggle.isOn = true;
	//		PlayerPrefs.SetInt("Headbobber", 1);
	//		PlayerPrefs.Save();
	//	}


	//}

	void Start()
	{
		bool isHeadbobberOn = (PlayerPrefs.GetInt("Headbobber") == 0) ? true : false;
		m_Headbobber.GetComponent<Veikkos_Headbobber>().enabled = isHeadbobberOn;
		m_HeadbobberToggle.isOn = isHeadbobberOn;
		

	}

	public void Headbobbing()
	{
		switch (m_HasBeenToggled)
		{
			case true:
				m_HasBeenToggled = false;
				m_Headbobber.GetComponent<Veikkos_Headbobber>().enabled = false;
				m_HeadbobberToggle.isOn = false;
				//Save System for bool
				PlayerPrefs.SetInt("Headbobber", 0);
				PlayerPrefs.Save();

				break;

			case false:
				m_HasBeenToggled = true;
				m_Headbobber.GetComponent<Veikkos_Headbobber>().enabled = true;
				m_HeadbobberToggle.isOn = true;
				PlayerPrefs.SetInt("Headbobber", 1);
				PlayerPrefs.Save();

				break;


				//}

				//if (m_Headbobber.GetComponent<Veikkos_Headbobber>().enabled == false)
				//{
				//	m_Headbobber.GetComponent<Veikkos_Headbobber>().enabled = true;
				//	m_HeadbobberToggle.isOn = true;
				//	PlayerPrefs.SetInt("Headbobber", 0);
				//	PlayerPrefs.Save();

				//	print("Headbobbing isON");

				//}
				//else
				//{
				//	m_Headbobber.GetComponent<Veikkos_Headbobber>().enabled = false;
				//	m_HeadbobberToggle.isOn = false;
				//	//Save System for bool
				//	PlayerPrefs.SetInt("Headbobber", 1);
				//	PlayerPrefs.Save();
				//	print("Headbobbing isOFF");

				}



		}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAMI_Shield : MonoBehaviour
{
	public PlayerAttack m_SwordAttack;
	public bool m_CanEquip;
	public GameObject m_Playerhand;
	public GameObject m_EquipShield;
	public StaminaBar m_Stamina;
	
	private void Start()
	{
		m_CanEquip = true;
		m_Stamina = FindObjectOfType<StaminaBar>();
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && m_SwordAttack.m_HasShield == false)
		{
			m_SwordAttack.m_HasShield = true;
			m_EquipShield.SetActive(true);
			print("Shield");
			//m_SwordAttack.SpawnShield();	

		}
		
	}
}

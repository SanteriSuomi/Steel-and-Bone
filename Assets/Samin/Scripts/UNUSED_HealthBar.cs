using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthBar : MonoBehaviour
{
	public Image currentHealthbar;
	public Text ratioText;
	private float hitpoint = 150;
	private float maxHitpoint = 150;
	//public Text m_EndText;
	private void Start()
	{
		UpdateHealthbar();
	}
	private void UpdateHealthbar()
	{
		float ratio = hitpoint / maxHitpoint;
		currentHealthbar.rectTransform.localScale = new Vector3(ratio, 1, 1);
		ratioText.text = (ratio*100).ToString() + '%';
		
	}
	private void TakeDamage(float damage)
	{
		hitpoint -= damage;
		if (hitpoint < 0)
		{
			hitpoint = 0;
			Debug.Log("Dead!");
			//if(gameObject.CompareTag("Player"))
			//if(gameObject.tag == "Player")
			//if (gameObject.name == "Player")
			{
			//	m_EndText.text = "You lose!";
			}
			//else if (gameObject.name == "Enemy")
			{
			//	m_EndText.text = "You Win!";
			}
			//else
			{
			//	print("This should not happen!");
			}
			//m_EndText.text = gameObject.name;
			//Destroy(gameObject); SceneManager.LoadScene(2);

		}
		UpdateHealthbar();
	}
	private void HealDamage(float heal)
	{
		hitpoint += heal;
		if (hitpoint > maxHitpoint)
			hitpoint = maxHitpoint;
		UpdateHealthbar();
	}



}
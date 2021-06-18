using Essentials.Saving;
using MessagePack;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SAMI_PlayerHealth : MonoBehaviour, ISaveable
{
	public float m_HealthPoints = 150f;
	public float m_CurrentHealth;
	public GameObject m_PlayerShield;
	public bool m_TakingDamage;
	public PlayerAttack m_SwORDaTTACK;
	public Slider m_HealthBar;
	public StaminaBar m_StaminaScript;
	public bool m_isPaused;
	public AudioSource m_HurtAudio;
	public AudioClip m_Hurt;
	public AudioClip m_BlockSound;
	public AudioSource m_HitByBoss;
	public AudioClip m_BossHitPlayerSFX;
	public SAMI_EnemyDamage m_Damage;
	public SAMI_PlayerStats m_Stats;
	public ParticleSystem m_BloodSplatter; // Players blood splatter when hit below half hp.
	private WaitForSeconds damageWaitForSeconds;

	private void Awake()
	{
		SaveSystem.Register(this);

		damageWaitForSeconds = new WaitForSeconds(1f);
		m_Damage = FindObjectOfType<SAMI_EnemyDamage>();
		m_Stats = FindObjectOfType<SAMI_PlayerStats>();
		m_HealthBar = GameObject.Find("HealthBar").GetComponent<Slider>();

		m_HealthPoints = 150;
		m_CurrentHealth = m_HealthPoints;
		m_HealthBar.value = m_HealthPoints;
		m_HealthBar.maxValue = m_HealthPoints;
	}

	#region Saving
	public SaveData GetSave()
	{
		return new PlayerHealthData(gameObject.name)
		{
			currentHealth = m_CurrentHealth,
			healthPoints = m_HealthPoints,
			healthbarCurrentValue = m_HealthBar.value,
			healthbarMaxValue = m_HealthBar.maxValue
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is PlayerHealthData save)
		{
			m_CurrentHealth = save.currentHealth;
			m_HealthPoints = save.healthPoints;
			m_HealthBar.value = save.healthbarCurrentValue;
			m_HealthBar.maxValue = save.healthbarMaxValue;
		}
	}

	[Serializable, MessagePackObject]
	public class PlayerHealthData : SaveData
	{
		[Key("Health")]
		public float healthPoints;
		[Key("CurrentHealth")]
		public float currentHealth;
		[Key("HealthbarMaxValue")]
		public float healthbarMaxValue;
		[Key("HealthbarCurrentValue")]
		public float healthbarCurrentValue;

		public PlayerHealthData() { }

		public PlayerHealthData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion

	private void Update()
	{
		if (m_CurrentHealth <= 0)
		{
			DeathEvent();
		}

		m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, -1, m_HealthPoints);
		m_HealthBar.value = m_CurrentHealth;
		m_HealthBar.maxValue = m_HealthPoints;
	}

	public void DeathEvent()
	{
		SceneManager.LoadScene("GameOver");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("EnemySword")
			|| (other.gameObject.CompareTag("Lvl4Enemy")
			&& !m_SwORDaTTACK.m_HasShieldUp
			&& !m_TakingDamage))
		{
			if (PlayerAttack.IsBlocking)
			{
				Vector3 enemyForward = other.GetComponentInParent<EnemyHealthBar>().transform.forward;
				Vector3 playerForward = transform.forward;
				if (Vector3.Dot(enemyForward, playerForward) <= -0.35)
				{
					StartCoroutine(MeleeEnemyDamageBlocked());
					m_HurtAudio.volume = Random.Range(0.8f, 1f);
					m_HurtAudio.PlayOneShot(m_BlockSound);
				}
			}
			else
			{
				if (other.gameObject.CompareTag("Lvl4Enemy"))
				{
					StartCoroutine(EnemyLvl4Damage());
					m_HurtAudio.volume = Random.Range(0.7f, 0.9f);
					m_HurtAudio.PlayOneShot(m_Hurt);
					PlayBloodSplatter();
				}
				else
				{
					StartCoroutine(MeleeEnemyDamage());
					m_HurtAudio.volume = Random.Range(0.7f, 0.9f);
					m_HurtAudio.PlayOneShot(m_Hurt);
					PlayBloodSplatter();
				}
			}
		}
		else if (other.gameObject.CompareTag("Fireball") && !m_TakingDamage)
		{
			m_TakingDamage = true;
			m_CurrentHealth -= m_Damage.m_FireballDamage;
			m_HurtAudio.volume = Random.Range(0.7f, 0.9f);
			m_HurtAudio.PlayOneShot(m_Hurt);
			Invoke("TookFireDamage", 0.25f);
		}
		else if (other.gameObject.CompareTag("BossAOE"))
		{
			m_CurrentHealth -= m_Damage.m_BossAoeDamage;

		}
		else if (other.gameObject.CompareTag("BossSingleAttack") && !m_TakingDamage)
		{
			StartCoroutine(BossMeleeDamage());
			m_HurtAudio.volume = Random.Range(0.3f, 0.5f);
			m_HurtAudio.PlayOneShot(m_Hurt);
			m_HitByBoss.PlayOneShot(m_BossHitPlayerSFX);
			PlayBloodSplatter();
		}
		else if (other.gameObject.CompareTag("BOSSJump"))
		{
			m_CurrentHealth -= m_Damage.m_BossDamage;

		}
		else if (other.gameObject.CompareTag("BossSpin") && !m_TakingDamage)
		{
			StartCoroutine(BossSpinDamage());
			PlayBloodSplatter();
		}
	}

	private void PlayBloodSplatter()
	{
		if (m_CurrentHealth <= 160)
		{
			m_BloodSplatter.Play();
			Invoke(nameof(BloodSplatterDelay), 0.35f);
		}
	}

	private void BloodSplatterDelay()
	{
		m_BloodSplatter.Stop();
	}

	private IEnumerator EnemyLvl4Damage()
	{
		m_TakingDamage = true;
		m_CurrentHealth -= m_Damage.m_Lvl4Enemy;
		yield return damageWaitForSeconds;
		m_TakingDamage = false;
	}

	private IEnumerator MeleeEnemyDamage()
	{
		m_TakingDamage = true;
		m_CurrentHealth -= m_Damage.m_Lvl1Enemy;
		yield return damageWaitForSeconds;
		m_TakingDamage = false;
	}

	private IEnumerator MeleeEnemyDamageBlocked()
	{
		m_StaminaScript.DrainStamina(50f);
		m_TakingDamage = true;
		yield return damageWaitForSeconds;
		m_TakingDamage = false;
	}

	private IEnumerator BossMeleeDamage()
	{
		m_TakingDamage = true;
		m_CurrentHealth -= m_Damage.m_BossDamage;
		yield return damageWaitForSeconds;
		m_TakingDamage = false;
	}

	private IEnumerator BossSpinDamage()
	{
		m_TakingDamage = true;
		m_CurrentHealth -= m_Damage.m_BossSpinDamage;
		yield return damageWaitForSeconds;
		m_TakingDamage = false;
	}

	private void TookFireDamage()
	{
		m_TakingDamage = false;
	}
}
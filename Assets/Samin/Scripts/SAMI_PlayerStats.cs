using Essentials.Saving;
using MessagePack;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SAMI_PlayerStats : MonoBehaviour, ISaveable
{
	public int m_CurrentLevel;
	public int m_CurrentExp;
	public int[] m_ToLevelUp;
	public Text m_LevelUpText;
	public SAMI_PlayerHealth m_HealthIncrease;
	public float m_MaxHealth;
	public int m_AttackLevel;
	public int m_DefenceLevel;
	public PlayerAttack m_Damage;
	public int m_CurrentAttackLevel;
	public int m_CurrentDefenceLevel;
	public SAMI_EnemyDamage m_EnemyDamage;
	public PlayerCharacterController m_CharController;
	private Slider m_PlayerHealthSlider;
	private AbilityBarHandler m_AbilityHandler;

	private void Awake()
	{
		SaveSystem.Register(this);

		m_MaxHealth = 150f;
		m_CharController = FindObjectOfType<PlayerCharacterController>();
		m_Damage = m_CharController.GetComponent<PlayerAttack>();
		m_HealthIncrease = m_CharController.GetComponent<SAMI_PlayerHealth>();
		m_PlayerHealthSlider = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<Slider>();
		m_AbilityHandler = FindObjectOfType<AbilityBarHandler>();
	}

	#region Saving
	public SaveData GetSave()
	{
	    return new PlayerStatsData(gameObject.name)
		{
			level = m_CurrentLevel,
			experience = m_CurrentExp,
			maxHealth = m_MaxHealth,
			levelText = m_LevelUpText.text,
			attackLevel = m_AttackLevel,
			defenseLevel = m_DefenceLevel,
			currentDefenseLevel = m_CurrentDefenceLevel,
			currentAttackLevel = m_CurrentAttackLevel
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is PlayerStatsData save)
		{
			m_CurrentLevel = save.level;
			m_CurrentExp = save.experience;
			m_MaxHealth = save.maxHealth;
			m_LevelUpText.text = save.levelText;
			m_AttackLevel = save.attackLevel;
			m_DefenceLevel = save.defenseLevel;
			m_CurrentDefenceLevel = save.currentDefenseLevel;
			m_CurrentAttackLevel = save.currentAttackLevel;
		}

		m_AbilityHandler.UpdateAbilityBar();
	}

	[Serializable, MessagePackObject]
	public class PlayerStatsData : SaveData
	{
		[Key("Level")]
		public int level;
		[Key("Experience")]
		public int experience;
		[Key("MaxHealth")]
		public float maxHealth;
		[Key("LevelText")]
		public string levelText;
		[Key("AttackLevel")]
		public int attackLevel;
		[Key("DefenseLevel")]
		public int defenseLevel;
		[Key("CurrentAttackLevel")]
		public int currentAttackLevel;
		[Key("CurrentDefenceLevel")]
		public int currentDefenseLevel;

		public PlayerStatsData() { }

		public PlayerStatsData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion

	private void Update()
	{
		if (m_CurrentLevel <= m_ToLevelUp.GetUpperBound(0) 
			&& m_CurrentExp >= m_ToLevelUp[m_CurrentLevel])
		{
			m_CurrentLevel++;
			Mathf.Clamp(m_CurrentLevel, 1f, 5f);
			m_AttackLevel++;
			Mathf.Clamp(m_AttackLevel, 1f, 14f);
			m_DefenceLevel++;
			Mathf.Clamp(m_DefenceLevel, 1f, 14f);
			m_Damage.m_DamageDone += 1 + (m_CurrentAttackLevel / 2);
			m_EnemyDamage.m_Lvl1Enemy -= m_CurrentDefenceLevel / 2f;
			m_EnemyDamage.m_BossDamage -= m_CurrentDefenceLevel / 2f;
			m_EnemyDamage.m_BossAoeDamage -= m_CurrentDefenceLevel / 2f;
			m_EnemyDamage.m_BossSpinDamage -= m_CurrentDefenceLevel / 2f;
			m_EnemyDamage.m_FireballDamage -= m_CurrentDefenceLevel / 2f;
			m_EnemyDamage.m_Lvl4Enemy -= m_CurrentDefenceLevel / 2f;
			m_PlayerHealthSlider.maxValue += 10;
			m_HealthIncrease.m_HealthPoints += 10f;
			m_MaxHealth += 10f;
			m_AbilityHandler.UpdateAbilityBar();
		}

		m_LevelUpText.text = "Lvl: " + m_CurrentLevel;
		m_CurrentAttackLevel = m_AttackLevel;
		m_CurrentDefenceLevel = m_DefenceLevel;
	}

	public void AddExperience(int experienceToAdd)
	{
		m_CurrentExp += experienceToAdd;
	}
}
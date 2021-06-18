using Essentials.Saving;
using MessagePack;
using System;
using UnityEngine;

public class SAMI_EnemyDamage : MonoBehaviour, ISaveable
{
	public float m_Lvl1Enemy = 20f;
	public float m_BossDamage = 12.5f;
	public float m_BossAoeDamage = 27.5f;
	public float m_BossSpinDamage = 17.5f;
	public float m_Lvl4Enemy = 40f;
	public float m_FireballDamage = 20f;

	private void Awake()
	{
		SaveSystem.Register(this);
	}

	#region Saving
	public SaveData GetSave()
	{
		return new EnemyDamageData(gameObject.name)
		{
			lvl1EnemyDamage = m_Lvl1Enemy,
			bossDamage = m_BossDamage,
			bossAoeDamage = m_BossAoeDamage,
			bossSpinDamage = m_BossSpinDamage,
			lvl4EnemyDamage = m_Lvl4Enemy,
			fireballDamage = m_FireballDamage
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is EnemyDamageData save)
		{
			m_Lvl1Enemy = save.lvl1EnemyDamage;
			m_BossDamage = save.bossDamage;
			m_BossAoeDamage = save.bossAoeDamage;
			m_BossSpinDamage = save.bossSpinDamage;
			m_Lvl4Enemy = save.lvl4EnemyDamage;
			m_FireballDamage = save.fireballDamage;
		}
	}

	[Serializable, MessagePackObject]
	public class EnemyDamageData : SaveData
	{
		[Key("Lvl1EnemyDamage")]
		public float lvl1EnemyDamage;
		[Key("BossDamage")]
		public float bossDamage;
		[Key("BossAoeDamage")]
		public float bossAoeDamage;
		[Key("BossSpinDamage")]
		public float bossSpinDamage;
		[Key("Lvl4EnemyDamage")]
		public float lvl4EnemyDamage;
		[Key("FireballDamage")]
		public float fireballDamage;

		public EnemyDamageData() { }

		public EnemyDamageData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion
}
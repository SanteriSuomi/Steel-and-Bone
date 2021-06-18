using Essentials.Saving;
using MessagePack;
using System;
using UnityEngine;

public class SAMI_BossDoor : MonoBehaviour, ISaveable
{
	private BossRanged boss;
	[SerializeField] Animator m_AnimatorDoor = default;
	[SerializeField] Animator m_AnimatorDoorB = default;
	//[SerializeField] GameObject m_Key;
	public PlayerAttack m_SwordAttack = default;
	public Collider m_NormalCollider = default;
	public Collider m_NormalColliderB = default;
	[SerializeField] Collider m_StopPlayer = default;
	[SerializeField]
	private BossDoorStopper bossDoorStopper = default;
	[SerializeField]
	private GameObject bossDoorKey = default;

	[SerializeField]
	private BossAreaMusic bossAreaMusic = default;
	public BossAreaMusic BossAreaMusic => bossAreaMusic;
	[SerializeField]
	private ExitBossArea exitBossAreaMusic = default;
	public ExitBossArea ExitBossArea => exitBossAreaMusic;

	private void Awake()
	{
		SaveSystem.Register(this);

		m_SwordAttack = FindObjectOfType<PlayerAttack>();
		boss = FindObjectOfType<BossRanged>();
	}

	#region Saving
	public SaveData GetSave()
	{
		return new BossDoorData(gameObject.name)
		{
			isInBossRoom = bossDoorStopper.IsInBossRoom
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is BossDoorData save)
		{
			if (save.isInBossRoom)
			{
				SetBossFightState(new BossFightState(openDoor: false, enableCollider: true, enableBossMusic: false, isLoad: true));
			}
			else
			{
				SetBossFightState(new BossFightState(openDoor: true, enableCollider: false, enableBossMusic: true, isLoad: true));
			}

			if (m_SwordAttack.hasBossDoorKey)
			{
				bossDoorKey.SetActive(false);
			}
		}
	}

	[Serializable, MessagePackObject]
	public class BossDoorData : SaveData
	{
		[Key("IsInBossRoom")]
		public bool isInBossRoom;

		public BossDoorData() { }

		public BossDoorData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion

	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player")
			&& m_SwordAttack.hasBossDoorKey)
		{
			SetBossFightState(new BossFightState(openDoor: true, enableCollider: false, enableBossMusic: true, isLoad: false));
			CompleteFindDemonQuest();
		}
	}

	public void SetBossFightState(BossFightState state)
	{
		m_StopPlayer.enabled = false;
		boss.Healthbar.Activate();
		m_AnimatorDoor.SetBool("OpenBossDoor", state.OpenDoor);
		m_AnimatorDoorB.SetBool("OpenBossDoor", state.OpenDoor);
		m_NormalCollider.enabled = state.EnableCollider;
		m_NormalColliderB.enabled = state.EnableCollider;
		if (state.EnableBossMusic)
		{
			bossAreaMusic.ActivateBossMusic();
		}
		else if (!state.IsLoad)
		{
			exitBossAreaMusic.StartNormalMusic();
		}
	}

	private static void CompleteFindDemonQuest()
	{
		QuestContext demonDoorQuest = QuestManager.Instance.GetQuest(4);
		demonDoorQuest.Data.IsCompleted = true;
	}
}

public struct BossFightState
{
	public bool OpenDoor { get; }
	public bool EnableCollider { get; }
	public bool EnableBossMusic { get; }
	public bool IsLoad { get; }

	public BossFightState(bool openDoor, bool enableCollider, bool enableBossMusic, bool isLoad)
	{
		OpenDoor = openDoor;
		EnableCollider = enableCollider;
		EnableBossMusic = enableBossMusic;
		IsLoad = isLoad;
	}
}
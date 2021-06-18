using Essentials.Saving;
using MessagePack;
using System;
using UnityEngine;

public class SAMI_KeyChecker : MonoBehaviour, ISaveable
{
	[SerializeField] Animator m_AnimatorDoor = default;
	[SerializeField] Animator m_AnimatorDoorB = default;

	private AudioSource m_DoorAudioSource;

	private Collider doorCollider;
	private Collider doorColliderB;
	private Collider doorTrigger;

	[SerializeField]
	private GameObject keyImage = default;

	private void Awake()
	{
		SaveSystem.Register(this);

		doorCollider = m_AnimatorDoor.GetComponent<Collider>();
		doorColliderB = m_AnimatorDoorB.GetComponent<Collider>();
		m_DoorAudioSource = GetComponent<AudioSource>();
		doorTrigger = GetComponent<Collider>();
	}

	#region Saving
	public SaveData GetSave()
	{
		return new KeyCheckerData(gameObject.name)
		{
			// Don't need save data
			//mazeKeyQuestCompleted = QuestManager.Instance.GetQuest(id: 1).Data.IsCompleted
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is KeyCheckerData save)
		{
			Invoke(nameof(CheckQuestCompleted), 0.5f);
		}
	}

	private void CheckQuestCompleted()
	{
		QuestContext findDoorQuest = QuestManager.Instance.GetQuest(id: 1);
		if (findDoorQuest.Data.IsCompleted)
		{
			OpenDoor();
		}
	}

	[Serializable, MessagePackObject]
	public class KeyCheckerData : SaveData
	{
		[Key("MazeKeyQuestState")]
		public bool mazeKeyQuestCompleted;

		public KeyCheckerData() { }

		public KeyCheckerData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion

	private void OnTriggerEnter(Collider other)
	{
		QuestContext mazeKeyQuest = QuestManager.Instance.GetQuest(id: 0);
		if (mazeKeyQuest.Data.IsCompleted
			&& other.gameObject.CompareTag("Player"))
		{
			OpenDoor();
		}
	}

	private void OpenDoor()
	{
		enabled = true;
		m_DoorAudioSource.Play();
		m_AnimatorDoor.SetBool("Open", true);
		m_AnimatorDoorB.SetBool("Open", true);
		doorCollider.enabled = false;
		doorColliderB.enabled = false;
		keyImage.SetActive(false);
		doorTrigger.enabled = false;
		QuestContext findDoorQuest = QuestManager.Instance.GetQuest(1);
		findDoorQuest.Data.IsCompleted = true;
		enabled = false;
	}
}
using Essentials.Saving;
using MessagePack;
using System;
using UnityEngine;

public class PuzzleEnd : MonoBehaviour, ISaveable
{
    [SerializeField] private WestTarget WestTarget = default;
    [SerializeField] private EastTarget EastTarget = default;
    [SerializeField] private SouthTarget SouthTarget = default;
    [SerializeField] private GameObject m_Wall = default;

	private bool wallOpened;

	private void Awake()
	{
		SaveSystem.Register(this);
	}

	private void Update()
    {
        if (WestTarget.WestTriggered && EastTarget.EastTriggered && SouthTarget.SouthTriggered
			&& !wallOpened)
		{
			OpenWall();
		}
		else
        {
            m_Wall.SetActive(true);
        }
    }

	#region Saving
	public SaveData GetSave()
	{
		return new PuzzleEndData(gameObject.name)
		{
			wallOpened = wallOpened
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is PuzzleEndData save
			&& save.wallOpened)
		{
			OpenWall();
		}
	}

	[Serializable, MessagePackObject]
	public class PuzzleEndData : SaveData
	{
		[Key("WallOpened")]
		public bool wallOpened;

		public PuzzleEndData() { }

		public PuzzleEndData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion

	private void OpenWall()
	{
		wallOpened = true;
		QuestContext secondPuzzleCompletedQuest = QuestManager.Instance.GetQuest(15);
		secondPuzzleCompletedQuest.Data.IsCompleted = true;
		AchievementManager.Activate(Achievements.Get("PUZZLE_2_COMPLETED"));
		m_Wall.SetActive(false);
		enabled = false;
	}
}
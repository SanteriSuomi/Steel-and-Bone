using Essentials.Saving;
using MessagePack;
using System;
using UnityEngine;

/// <summary>
/// QuestContext is needed for scene references for the quest.
/// </summary>
public class QuestContext : MonoBehaviour, ISaveable
{
    [SerializeField]
    private QuestData data = default;
    public QuestData Data => data;

    [SerializeField]
    private Transform location = default;
    public Transform Location
	{
		get => location;
		set
		{
			location = value;
		}
	}

    private void Awake()
    {
        SaveSystem.Register(this);

        // Register this scene context to the data asset.
        data.Context = this;
    }

    #region Saving
    public SaveData GetSave()
	{
		return new QuestSaveData(gameObject.name)
		{
			isCompleted = data.IsCompleted
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is QuestSaveData save)
		{
			data.IsCompleted = save.isCompleted;
		}
	}

	[Serializable, MessagePackObject]
	public class QuestSaveData : SaveData
	{
		[Key("IsCompleted")]
		public bool isCompleted;

		public QuestSaveData() { }

		public QuestSaveData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion
}
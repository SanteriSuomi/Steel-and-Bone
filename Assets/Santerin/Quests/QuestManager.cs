using Essentials.Saving;
using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>, ISaveable, ISerializationCallbackReceiver
{
    private AudioSource questCompleteAudSrc;
    private Dictionary<string, QuestContext> questDatabase;

    [Tooltip("First quest in the list is the starting quest and last is the ending quest.")]
    private List<QuestContext> quests;
    private QuestContext currentQuest;
    private QuestDisplayController displayController;
    [SerializeField, Tooltip("For debugging")]
    private int currentQuestIndex;

    private bool applicationIsPlaying;

    public void OnBeforeSerialize()
    {
        // Empty because not used right now
    }

    public void OnAfterDeserialize()
    {
        if (applicationIsPlaying)
        {
            currentQuest = quests[currentQuestIndex];
        }
    }

    protected override void Awake()
    {
        SaveSystem.Register(this);

        applicationIsPlaying = Application.isPlaying;

        displayController = FindObjectOfType<QuestDisplayController>();
        questCompleteAudSrc = GetComponent<AudioSource>();
        InitializeState();
        StartCoroutine(UpdateState());
    }

	#region Saving
	public SaveData GetSave()
	{
		return new QuestManagerData(gameObject.name)
		{
			currentQuestIndex = currentQuestIndex
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is QuestManagerData save)
		{
			currentQuestIndex = save.currentQuestIndex - 1;
		}
	}

	[Serializable, MessagePackObject]
	public class QuestManagerData : SaveData
	{
		[Key("QuestIndex")]
		public int currentQuestIndex;

		public QuestManagerData() { }

		public QuestManagerData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion

    private void InitializeState()
    {
        quests = new List<QuestContext>();
        GetComponentsInChildren(true, quests);
        questDatabase = new Dictionary<string, QuestContext>();
        for (int i = 0; i < quests.Count; i++)
        {
            // Title key
            questDatabase.Add(quests[i].Data.Title, quests[i]);
            // ID key
            questDatabase.Add(quests[i].Data.ID.ToString(), quests[i]);
            quests[i].Data.IsCompleted = false; // Make sure none of the quests are completed at the start.
        }

        SetNewCurrentQuest();
    }

    private IEnumerator UpdateState()
    {
        yield return new WaitForSeconds(1);
        while (enabled)
        {
            if (currentQuest.Data.TestCondition())
            {
                questCompleteAudSrc.Play();
                UpdateCurrentQuest();
            }

            yield return null;
        }
    }

    private void UpdateCurrentQuest()
    {
        StopCurrentQuest();
        currentQuestIndex++;
        if (currentQuestIndex < quests.Count // Move to the next quest
            && quests[currentQuestIndex] != null)
        {
            SetNewCurrentQuest();
        }
        else if (currentQuestIndex >= quests.Count
                 || quests[currentQuestIndex] == null)
        {
            displayController.DisableUpdateDisplay();
        }
        else
        {
            currentQuestIndex--;
        }
    }

    private void StopCurrentQuest()
    {
        displayController.StopUpdateDisplay();
    }

    private void SetNewCurrentQuest()
    {
        currentQuest = quests[currentQuestIndex];
        displayController.UpdateDisplay(currentQuest.Data.Title, currentQuest.Data.Description, currentQuest.Location);
        UpdateLocation(currentQuest.Location);
    }

    public QuestContext GetCurrentQuest()
    {
        return currentQuest;
    }

    public QuestContext GetQuest(string title)
    {
        return questDatabase[title];
    }

    public QuestContext GetQuest(int id)
    {
        string idString = id.ToString();
        return questDatabase[idString];
    }

    public void UpdateLocation(Transform location)
    {
        displayController.UpdateLocation(location);
    }
}
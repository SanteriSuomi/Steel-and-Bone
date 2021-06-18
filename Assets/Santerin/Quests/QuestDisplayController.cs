using Essentials.Saving;
using MessagePack;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class QuestDisplayController : MonoBehaviour, ISaveable
{
    [SerializeField]
    private TextMeshProUGUI questTitle = default;
    [SerializeField]
    private TextMeshProUGUI questDescription = default;
    [SerializeField]
    private TextMeshProUGUI questLocation = default;
    [SerializeField]
    private float distanceUpdateInterval = 1.5f;
    private Transform player;
    private Transform currentLocation;
    private WaitForSeconds updateDistanceWFS;
    private Coroutine updateDistanceCoroutine;
    private StringBuilder distanceStringBuilder;
    private int distanceStringBuilderIndex;

    private void Awake()
    {
        SaveSystem.Register(this);

        updateDistanceWFS = new WaitForSeconds(distanceUpdateInterval);
        distanceStringBuilder = new StringBuilder(questLocation.text);
        distanceStringBuilderIndex = distanceStringBuilder.Length;
    }

    private void Start()
    {
        player = EnemyGlobal.Instance.Player;
    }

    #region Saving
    public SaveData GetSave()
	{
		return new QuestDisplayData(gameObject.name)
		{
            title = questTitle.text,
            description = questDescription.text,
            location = questLocation.text
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is QuestDisplayData save)
		{
            questTitle.text = save.title;
            questDescription.text = save.description;
            questLocation.text = save.location;
		}
	}

	[Serializable, MessagePackObject]
	public class QuestDisplayData : SaveData
	{
		[Key("Title")]
		public string title;
        [Key("Description")]
        public string description;
        [Key("Location")]
        public string location;

		public QuestDisplayData() { }

		public QuestDisplayData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion

    /// <summary>
    /// Update current title, description and location. If title or description are empty, use the existing ones. If location is null, use the existing one.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="description"></param>
    /// <param name="location"></param>
    public void UpdateDisplay(string title, string description, Transform location)
    {
        questTitle.text = title;
        questDescription.text = description;
        currentLocation = location;

        if (location != null)
        {
            if (updateDistanceCoroutine != null)
            {
                StopCoroutine(updateDistanceCoroutine);
            }

            updateDistanceCoroutine = StartCoroutine(UpdateDistance());
        }
    }

    public void UpdateLocation(Transform location)
    {
        currentLocation = location;
    }

    private IEnumerator UpdateDistance()
    {
        if (player == null)
        {
            yield return new WaitForSeconds(1);
        }

        while (enabled)
        {
            string distanceString = GetUpdatedDistance(currentLocation);
            SetUpdatedDistance(distanceString);
            yield return updateDistanceWFS;
        }
    }

    private string GetUpdatedDistance(Transform location)
    {
        if (location == null) return string.Empty;
        float distance = Mathf.RoundToInt(Vector3.Distance(player.position, location.position));
        return distance.ToString();
    }

    private void SetUpdatedDistance(string distanceString)
    {
        distanceStringBuilder.Append(distanceString);
        questLocation.SetText(distanceStringBuilder);
        distanceStringBuilder.Remove(distanceStringBuilderIndex, distanceString.Length);
    }

    public void ActivateQuestDisplay()
    {
        questTitle.gameObject.SetActive(true);
        questDescription.gameObject.SetActive(true);
        questLocation.gameObject.SetActive(true);
    }

    public void StopUpdateDisplay()
    {
        if (updateDistanceCoroutine != null)
        {
            StopCoroutine(updateDistanceCoroutine);
        }

        questLocation.SetText(string.Empty);
    }

    public void DisableUpdateDisplay()
    {
        if (updateDistanceCoroutine != null)
        {
            StopCoroutine(updateDistanceCoroutine);
        }

        questTitle.SetText(string.Empty);
        questDescription.SetText(string.Empty);
        questLocation.SetText(string.Empty);
    }
}
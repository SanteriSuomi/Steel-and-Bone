using UnityEngine;

[CreateAssetMenu(fileName = "Quest Data", menuName = "Quest/Quest Data", order = 0)]
public class QuestData : ScriptableObject
{
    public bool IsCompleted { get; set; }

    private bool completedOnComplete;

    [SerializeField, Tooltip("By which ID can this quest be accessed from the quest database?")]
    private int id = default;
    public int ID => id;

    [SerializeField]
    private string title = default;
    public string Title => title;
    [SerializeField]
    private string description = default;
    public string Description => description;

    public QuestContext Context { get; set; }

    public delegate void ConditionCompleted(QuestEventArgs args);
    public event ConditionCompleted OnConditionCompletedEvent;

    /// <summary>
    /// Return true if the condition of this quest is completed. 
    /// Normally by setting IsCompleted to true, but can be overridden for extended behaviour.
    /// </summary>
    public virtual bool TestCondition()
    {
        if (IsCompleted)
        {
            if (!completedOnComplete)
            {
                completedOnComplete = true;
                OnConditionCompleted();
            }

            return true;
        }

        return false;
    }

    public virtual void OnConditionCompleted()
    {
        OnConditionCompletedEvent?.Invoke(new QuestEventArgs(this, Context));
    }
}
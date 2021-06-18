public class Achievement
{
    public string APIName { get; }
    public string DisplayName { get; }
    public string Description { get; }

    public Stat AssociatedStat { get; }
    public int StatThreshold { get; }

    public Achievement(string apiName, string displayName, string description, Stat associatedStat, int statThreshold)
    {
        APIName = apiName;
        DisplayName = displayName;
        Description = description;
        AssociatedStat = associatedStat;
        StatThreshold = statThreshold;
    }
}
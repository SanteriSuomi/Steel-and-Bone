using System;

public class QuestEventArgs : EventArgs
{
    public QuestData Data { get; }
    public QuestContext Context { get; }

    public QuestEventArgs(QuestData data, QuestContext context)
    {
        Data = data;
        Context = context;
    }
}
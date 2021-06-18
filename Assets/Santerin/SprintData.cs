using UnityEngine;

public struct SprintData
{
    public Vector3 Direction { get; }
    public float SprintMultiplier { get; }
    public float StaminaDrain { get; }

    public bool TransformDirection { get; }
    public bool AddGravity { get; }
    public bool MultiplyByDelta { get; }

    public SprintData(Vector3 direction, float sprintMultiplier, float staminaDrain,
        bool transformDirection, bool addGravity, bool multiplyByDelta)
    {
        Direction = direction;
        SprintMultiplier = sprintMultiplier;
        StaminaDrain = staminaDrain;
        TransformDirection = transformDirection;
        AddGravity = addGravity;
        MultiplyByDelta = multiplyByDelta;
    }
}
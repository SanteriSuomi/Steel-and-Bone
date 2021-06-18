using UnityEngine;

public class Viljo_PuzzleDoor : PuzzleDoorBase
{
	[SerializeField]
	private Transform puzzleDoorLocation = default;
	private Transform originalLocation;

	private QuestContext solveMazePuzzleQuest;

	protected override void Start()
	{
		base.Start();
		solveMazePuzzleQuest = QuestManager.Instance.GetQuest(2);
		originalLocation = solveMazePuzzleQuest.Location;
	}

	protected override void OnTriggerEnter(Collider _)
	{
		solveMazePuzzleQuest.Location = puzzleDoorLocation;
		QuestManager.Instance.UpdateLocation(solveMazePuzzleQuest.Location);
		DisableDoor();
	}

	protected override void OnTriggerExit(Collider _)
	{
		solveMazePuzzleQuest.Location = originalLocation;
		QuestManager.Instance.UpdateLocation(solveMazePuzzleQuest.Location);
		EnableDoor();
	}
}
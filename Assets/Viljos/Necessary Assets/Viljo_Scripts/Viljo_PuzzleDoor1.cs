using UnityEngine;

public class Viljo_PuzzleDoor1 : PuzzleDoorBase
{
	private Viljo_PuzzleDoor2 m_otherScript;
	public bool DestroyThisDoor1 { get; set; }

	protected override void Awake()
	{
		base.Awake();
		DestroyThisDoor1 = false;
		m_otherScript = FindObjectOfType<Viljo_PuzzleDoor2>();
	}

	protected override void OnTriggerEnter(Collider _)
	{
		DestroyThisDoor1 = true;
		if (m_otherScript.DestroyThisDoor2)
		{
			DisableDoor();
			AchievementManager.Activate(Achievements.Get("PUZZLE_COMPLETED"));
		}
	}

	protected override void OnTriggerExit(Collider _)
	{
		DestroyThisDoor1 = false;
		EnableDoor();
	}
}
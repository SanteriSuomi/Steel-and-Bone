using UnityEngine;

public class Viljo_PuzzleDoor2 : PuzzleDoorBase
{
	private Viljo_PuzzleDoor1 m_otherScript;
	public bool DestroyThisDoor2 { get; set; }

	protected override void Awake()
	{
		base.Awake();
		DestroyThisDoor2 = false;
		m_otherScript = FindObjectOfType<Viljo_PuzzleDoor1>();
	}

	protected override void OnTriggerEnter(Collider _)
	{
		DestroyThisDoor2 = true;
		if (m_otherScript.DestroyThisDoor1)
		{
			DisableDoor();
		}
	}

	protected override void OnTriggerExit(Collider _)
	{
		DestroyThisDoor2 = false;
		EnableDoor();
	}
}
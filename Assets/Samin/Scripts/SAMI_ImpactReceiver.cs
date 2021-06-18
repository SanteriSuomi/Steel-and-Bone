using UnityEngine;

public class SAMI_ImpactReceiver : MonoBehaviour
{
	float mass = 7.0F; 
	Vector3 impact = Vector3.zero;
	private CharacterController character;

	void Start()
	{
		character = GetComponent<CharacterController>();
	}

	void Update()
	{
		if (impact.magnitude > 0.2F) character.Move(impact * Time.deltaTime);
		impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
	}
	
	public void AddImpact(Vector3 dir, float force)
	{
		dir.Normalize();
		if (dir.y < 0) dir.y = -dir.y; 
		impact += dir.normalized * force / mass;
	}
}
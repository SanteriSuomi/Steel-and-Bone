using UnityEngine;
using UnityEngine.Events;

public class SafeHouse : MonoBehaviour
{
    public UnityEvent Trigger;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			Trigger.Invoke();
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTrigger : MonoBehaviour
{
	public UnityEvent Trigger;


	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			Trigger.Invoke();
		}
	}
}

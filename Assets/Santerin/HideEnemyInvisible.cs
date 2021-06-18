using UnityEngine;

public class HideEnemyInvisible : MonoBehaviour
{
	private EnemyBase enemy;
	private SkinnedMeshRenderer meshRenderer;

	private void Awake()
	{
		enemy = GetComponentInParent<EnemyBase>();
		meshRenderer = GetComponent<SkinnedMeshRenderer>();
	}

	private void OnBecameInvisible()
	{
		if (enemy.isDead)
		{
			Debug.Log("invisible");
			meshRenderer.enabled = false;
		}
	}

	private void OnBecameVisible()
	{
		if (enemy.isDead)
		{
			Debug.Log("visible");
			meshRenderer.enabled = true;
		}
	}
}
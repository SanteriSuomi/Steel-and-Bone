using UnityEngine;

public class EnemyMelee : EnemyBase
{
	protected override void UpdateState(float distanceToPlayer, float minDistanceForAttack)
	{
		minDistanceForAttack = EnemyGlobal.Instance.CurrentMinDistanceForAttack;
		base.UpdateState(distanceToPlayer, minDistanceForAttack);
	}

	#if UNITY_EDITOR
	protected override void OnDrawGizmos()
	{
		if (EnemyGlobal.Instance != null)
		{
			Gizmos.DrawWireSphere(transform.position, EnemyGlobal.Instance.CurrentMinDistanceForChase);
			Gizmos.DrawWireSphere(transform.position, EnemyGlobal.Instance.CurrentMinDistanceForAttack);
		}
	}
	#endif
}
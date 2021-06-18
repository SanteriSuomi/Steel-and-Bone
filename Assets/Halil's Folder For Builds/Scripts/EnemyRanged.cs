using UnityEngine;

public class EnemyRanged : EnemyBase
{
	protected override void UpdateState(float distanceToPlayer, float minDistanceForAttack)
	{
		minDistanceForAttack = EnemyGlobal.Instance.CurrentMinDistanceForAttackRanged;
		base.UpdateState(distanceToPlayer, minDistanceForAttack);
	}

	#if UNITY_EDITOR
	protected override void OnDrawGizmos()
	{
		if (EnemyGlobal.Instance != null)
		{
			Gizmos.DrawWireSphere(transform.position, EnemyGlobal.Instance.CurrentMinDistanceForChase);
			Gizmos.DrawWireSphere(transform.position, EnemyGlobal.Instance.CurrentMinDistanceForAttackRanged);
		}
	}
	#endif
}
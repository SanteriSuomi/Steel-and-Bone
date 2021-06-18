using System.Collections;
using UnityEngine;

public class EnemyMiniBoss : EnemyBase
{
    [SerializeField]
    private GameObject m_Fireball = default;
    [SerializeField]
    private Transform[] fireballPositions = default;

    [SerializeField]
    private float specialAttackCooldown = 10;
    private WaitForSeconds specialAttackWFS;

    private bool m_HasUsedSpecialAttack;

    protected override void Awake()
    {
        base.Awake();
        specialAttackWFS = new WaitForSeconds(specialAttackCooldown);
    }

    protected override void UpdateState(float distanceToPlayer, float minDistanceForAttack)
    {
        minDistanceForAttack = EnemyGlobal.Instance.CurrentMinDistanceForAttack;
        base.UpdateState(distanceToPlayer, minDistanceForAttack);
    }

    protected override IEnumerator SpecialAttack()
    {
        if (m_HasUsedSpecialAttack) yield break;
        m_HasUsedSpecialAttack = true;
        Invoke(nameof(FireBallSound), 0.15f);
        Invoke(nameof(FireBallSound), 0.55f);
        for (int i = 0; i < fireballPositions.Length; i++)
        {
            Instantiate(m_Fireball, fireballPositions[i].position, fireballPositions[i].localRotation);
        }

        yield return specialAttackWFS;
        m_HasUsedSpecialAttack = false;
    }

    private void FireBallSound()
    {
        audiosource.PlayOneShot(fireBallSound);
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
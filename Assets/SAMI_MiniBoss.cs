using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SAMI_MiniBoss : MonoBehaviour
{
    public Animator m_Animator;
    public PlayerAttack m_SwordAttack;
    public float m_MeleeRange = 6f;
    public int m_MiniBossHealth = 150;
    private bool m_Dead = false;
    public Rigidbody m_PlayerRigidBody;
    public float m_ChaseRange = 14f;
    public bool m_MeleeMode = true;
    public GameObject m_MiniBossAoe;
    public SAMI_InstantiateMiniBossAoe m_InstantiateMiniBossAoe;
    public Collider m_AttackCollider;
    public NavMeshAgent m_NavMeshAgent;
    public bool m_IsAttacking = false;
    public AudioClip enemyHitSound;
    public AudioClip enemyHitSoundTwo;
    public AudioClip enemyFootStep;
    public AudioClip enemyKickedSound;
    public AudioClip enemyDeathSound;
    public AudioClip fireBallSound;
    public AudioSource m_AudioSource1;
    void Start()
    {
        m_SwordAttack = FindObjectOfType<PlayerAttack>();
        m_PlayerRigidBody = m_SwordAttack.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword"))
        {
            m_MiniBossHealth -= m_SwordAttack.m_DamageDone;
        }
    }

    public void EnemyAttackColliderEnable()
    {
        m_AttackCollider.enabled = true;
    }

    public void EnemyAttackColliderDisable()
    {
        m_AttackCollider.enabled = false;
    }



    // Update is called once per frame
    void Update()
    {
        if(m_MiniBossHealth <= 0)
        {
            m_Dead = true;
        }

        if (Vector3.Distance(transform.position, m_PlayerRigidBody.position) > m_MeleeRange && m_MiniBossHealth <= 250 && !m_Dead)           
        {
            StartCoroutine(MiniBossAreaAttack());         
        }
        if (Vector3.Distance(transform.position, m_PlayerRigidBody.position) < m_MeleeRange && !m_Dead && m_IsAttacking == false)
        {
            StartCoroutine(Attack());
        }
        if (Vector3.Distance(transform.position, m_PlayerRigidBody.position) > m_MeleeRange && !m_Dead && m_IsAttacking == false)
        {
            Chase();
        }






    }

    public void EnemeyFootStep()
    {
        m_AudioSource1.PlayOneShot(enemyFootStep);
    }


    IEnumerator Attack()
    {
        m_IsAttacking = true;
        m_Animator.SetBool("Idle", false);
        m_Animator.SetBool("Walk", false);
        m_Animator.SetBool("isHit", false);
        m_Animator.SetBool("Dead", false);
        m_Animator.SetBool("Attack", true);
        m_Animator.SetBool("Kicked", false);
        m_NavMeshAgent.speed = 0f;
        yield return new WaitForSeconds(2f);
        m_Animator.SetBool("Idle", true);
        m_Animator.SetBool("Walk", false);
        m_Animator.SetBool("isHit", false);
        m_Animator.SetBool("Dead", false);
        m_Animator.SetBool("Attack", false);
        m_Animator.SetBool("Kicked", false);
        m_IsAttacking = false;
    }

    public void Chase()
    {
        m_NavMeshAgent.destination = m_PlayerRigidBody.position;
        m_NavMeshAgent.speed = 3f;
        m_Animator.SetBool("Idle", false);
        m_Animator.SetBool("Walk", true);
        m_Animator.SetBool("isHit", false);
        m_Animator.SetBool("Dead", false);
        m_Animator.SetBool("Attack", false);
        m_Animator.SetBool("Kicked", false);
    }


    IEnumerator MiniBossAreaAttack()
    {
        m_MiniBossAoe.gameObject.SetActive(true);
        m_InstantiateMiniBossAoe.ExecuteAoe();
        yield return new WaitForSeconds(2f);
        m_MiniBossAoe.gameObject.SetActive(false);
    }



#if UNITY_EDITOR
    private void OnDrawGizmos()
    {        
        Gizmos.DrawWireSphere(transform.position, m_MeleeRange);
        Gizmos.DrawWireSphere(transform.position, m_ChaseRange);
    }
#endif
}

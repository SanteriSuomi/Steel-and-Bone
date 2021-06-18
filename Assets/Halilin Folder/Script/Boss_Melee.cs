using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Melee : MonoBehaviour
{
    public Transform Player;

    Animator animator;
    
    

    public bool bossMeleeAttacking = false;
    public bool tauntDone = false;

    public float meleeRange = 7.5f;
    public float roarRange = 15f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, Player.position) < meleeRange && tauntDone == true)
        {
            StartCoroutine(BossAttack());
        }
        else if (Vector3.Distance(transform.position, Player.position) < roarRange && tauntDone == false && bossMeleeAttacking == false)
        {
            StartCoroutine(BossTaunt());

        }
        else if (Vector3.Distance(transform.position, Player.position) > meleeRange && bossMeleeAttacking == false && tauntDone == true)
        {
            animator.SetBool("meleeAttack", false);
            animator.SetBool("bossRoar", false);
            animator.SetBool("Idle", true);
        }
        
    }

    IEnumerator BossAttack()
    {
        bossMeleeAttacking = true;
        animator.SetBool("meleeAttack", true);
        animator.SetBool("bossRoar", false);
        animator.SetBool("Idle", false);
        yield return new WaitForSeconds(3.25f);
        bossMeleeAttacking = false;
        
    }

    IEnumerator BossTaunt()
    {
        
        animator.SetBool("meleeAttack", false);
        animator.SetBool("bossRoar", true);
        animator.SetBool("Idle", false);
        yield return new WaitForSeconds(5.5f);
        tauntDone = true;
        animator.SetBool("Idle", true);
        animator.SetBool("bossRoar", false);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.DrawWireSphere(transform.position, roarRange);
    }

}

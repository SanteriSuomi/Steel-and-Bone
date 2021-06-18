using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MageEnemy : MonoBehaviour
{
    Animator animator;

    private bool readyToShoot = true;
    public bool dead = false;

    public Collider enemyCollider;
    NavMeshAgent agent;
    // public GameObject fireBall;
    private Transform Player = default;

    public float HealthPoints = 100f;

    //public PlayerMovement playermovement;
    public ShootFireBall shootFireBall;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        dead = false;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, Player.position) < 10f && !dead)
        {
            Vector3 targetPosition = new Vector3(Player.position.x, transform.position.y, Player.position.z);
            transform.LookAt(targetPosition);
            if (readyToShoot)
            {
                StartCoroutine(ShootingAnimations());
            }
        }

        if (HealthPoints <= 0 && dead == false)
        {
            animator.SetBool("Dead", true);
            dead = true;
            //playermovement.doneWithEnemy = true;
            agent.enabled = false;
            enemyCollider.enabled = false;
        }
    }

    IEnumerator ShootingAnimations()
    {

        animator.SetBool("Shoot", true);
        animator.SetBool("Idle", false);

        readyToShoot = false;
        yield return new WaitForSeconds(2f);
        animator.SetBool("Shoot", false);
        animator.SetBool("Idle", true);

        readyToShoot = true;
    }

    public void ShootFireBall() // This is the only important part here
    {
        shootFireBall.FireBall();

    }
}
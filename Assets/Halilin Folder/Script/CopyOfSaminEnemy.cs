using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CopyOfSaminEnemy : MonoBehaviour
{

	public Transform Player;
	public Transform Waypoint;
	private SAMI_PlayerStats m_PlayerStats;
	public Collider EnemyCollider;

	public int m_ExpToGive;
	NavMeshAgent agent;
	Animator animator;
	[SerializeField] private Collider m_EnemySword = default;

	//[SerializeField] float decisionDelay = 0.25f;
	[SerializeField] Transform[] waypoints = default;
	int currentWaypoint = 0;

	public bool inSafeHouse = false;
	public bool isCaught = false;
	public bool isHit = false;
	public bool isKicked = false;
	public bool isDead = false;
	public bool isWaiting = false;
	public bool playerHasAttacked = false;
	public bool attacking = false;
	public bool hasPlayed = false;
	public float m_caughtDistance = 3f;
	public float m_chaseDistance = 15f;

	public float m_Healthpoint = 30f;
	public PlayerAttack m_SwordAttack;
	public GameObject m_HealthPotion;
	public float m_DropChance = 0.5f;
	public SAMI_EnemyDamage m_Damage;

	// Start is called before the first frame update
	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		m_Damage = FindObjectOfType<SAMI_EnemyDamage>();
		m_PlayerStats = FindObjectOfType<SAMI_PlayerStats>();
		m_EnemySword.enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (m_Healthpoint <= 0)
		{

			if (Random.Range(0f, 1f) <= m_DropChance && isDead == false)
			{
				Instantiate(m_HealthPotion, transform.position, transform.rotation);
			}
			if (isDead == false)
			{
				m_PlayerStats.AddExperience(m_ExpToGive);

			}

			StartCoroutine(EnemyDeath());
		}

		if (Vector3.Distance(transform.position, Player.position) < m_caughtDistance && isHit == false && isKicked == false && inSafeHouse == false)
		{

			StartCoroutine(AttackingPlayer());
		}
		else if (Vector3.Distance(transform.position, Player.position) < m_chaseDistance && isHit == false && attacking == false && isKicked == false && inSafeHouse == false)
		{
			ChasePlayer();
		}
		else if (Vector3.Distance(transform.position, Waypoint.position) < 0.35f && isHit == false && attacking == false && isKicked == false)
		{
			EnemyIdle();
		}
		else if (inSafeHouse == true && isWaiting == false && isKicked == false && attacking == false && isHit == false)
		{
			StartPatrolling();
		}

		else if (Vector3.Distance(transform.position, Player.position) > m_chaseDistance && playerHasAttacked == false && inSafeHouse == false)
		{

			StartPatrolling();

		}
		else if (playerHasAttacked == true && isKicked == false && attacking == false && isHit == false && inSafeHouse == false)
		{
			ChasePlayer();
		}
	}

	IEnumerator AttackingPlayer()
	{
		isWaiting = false;
		attacking = true;
		agent.speed = 0f;

		Vector3 targetPosition = new Vector3(Player.position.x, this.transform.position.y, Player.position.z);
		this.transform.LookAt(targetPosition);

		animator.SetBool("Attack", true);
		animator.SetBool("Walk", false);
		//animator.SetBool("Patrol", false);
		animator.SetBool("Idle", false);
		animator.SetBool("Kicked", false);
		animator.SetBool("isHit", false);
		yield return new WaitForSeconds(2.65f);
		attacking = false;
	}
	void ActivateSword()
	{
		m_EnemySword.enabled = true;
	}
	void DeActivateSword()
	{
		m_EnemySword.enabled = false;
	}

	public void ChasePlayer()
	{
		isWaiting = false;

		//Follow the player
		agent.destination = Player.position;
		agent.speed = 3f;


		animator.SetBool("Walk", true);
		animator.SetBool("Idle", false);
		animator.SetBool("isHit", false);
		//animator.SetBool("Patrol", false);
		animator.SetBool("Kicked", false);
		animator.SetBool("Attack", false);
	}
	public void StartPatrolling()
	{
		if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) <= 0.6f)
		{
			currentWaypoint++;
			if (currentWaypoint == waypoints.Length)
			{
				currentWaypoint = 0;
			}
		}

		agent.SetDestination(waypoints[currentWaypoint].position);

		agent.speed = 1f;

		//animator.SetBool("Patrol", true);
		animator.SetBool("Walk", true);
		animator.SetBool("isHit", false);
		animator.SetBool("Idle", false);
		animator.SetBool("Kicked", false);
		animator.SetBool("Attack", false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Sword"))
		{
			print("Sword Detected");
			StartCoroutine(GetHit());
		}
		else if (other.gameObject.CompareTag("Foot"))
		{
			StartCoroutine(GetKicked());
		}
		else if (other.gameObject.CompareTag("LightAttackSword"))
		{
			StartCoroutine(LightAttackHit());
		}
	}


	IEnumerator GetHit()
	{
		m_Healthpoint -= m_SwordAttack.m_DamageDone;
		attacking = false;
		playerHasAttacked = true;
		print("Got hit");
		isHit = true;
		agent.speed = 0f;
		//animator.SetBool("Patrol", false);
		animator.SetBool("Kicked", false);
		animator.SetBool("Walk", false);
		animator.SetBool("isHit", true);
		animator.SetBool("Idle", false);
		animator.SetBool("Attack", false);
		yield return new WaitForSeconds(2f);
		Debug.Log("Got Hit");
		isHit = false;

	}
	IEnumerator LightAttackHit()
	{
		attacking = false;
		isHit = true;
		agent.speed = 0;
		playerHasAttacked = true;
		m_Healthpoint -= m_SwordAttack.m_DamageDone - 7;
		//animator.SetBool("Patrol", false);
		animator.SetBool("Kicked", false);
		animator.SetBool("Walk", false);
		animator.SetBool("isHit", true);
		animator.SetBool("Idle", false);
		animator.SetBool("Attack", false);
		yield return new WaitForSeconds(0.65f);
		isHit = false;
	}

	IEnumerator GetKicked()
	{
		attacking = false;
		playerHasAttacked = true;
		isKicked = true;
		agent.speed = 0f;
		animator.SetBool("Kicked", true);
		//animator.SetBool("Patrol", false);
		animator.SetBool("Walk", false);
		animator.SetBool("isHit", false);
		animator.SetBool("Idle", false);
		animator.SetBool("Attack", false);
		yield return new WaitForSeconds(1.6f);
		isKicked = false;
	}


	IEnumerator EnemyDeath()
	{
		isDead = true;

		animator.SetBool("Dead", true);
		animator.SetBool("Attack", false);
		animator.SetBool("Walk", false);
		//animator.SetBool("Patrol", false);
		animator.SetBool("Idle", false);
		animator.SetBool("Kicked", false);
		animator.SetBool("isHit", false);

		agent.enabled = false;
		EnemyCollider.enabled = false;
		yield return new WaitForSeconds(100f);
		Destroy(gameObject);
	}

	public void EnteringSafeHouse()
	{
		inSafeHouse = true;
	}

	public void EnemyIdle()
	{
		isWaiting = true;
		animator.SetBool("Attack", false);
		animator.SetBool("Walk", false);
		//animator.SetBool("Patrol", false);
		animator.SetBool("Idle", true);
		animator.SetBool("Kicked", false);
		animator.SetBool("isHit", false);
	}

	public void LeavingSafeHouse()
	{
		inSafeHouse = false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, m_chaseDistance);
		Gizmos.DrawWireSphere(transform.position, m_caughtDistance);
	}

}

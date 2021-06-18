using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SaminEnemyKey : MonoBehaviour
{
	//public Transform otherEnemy;
	public Transform Player;
	public Transform Waypoint;
	public Transform lookAtWhenIdle;
	private SAMI_PlayerStats m_PlayerStats;
	public Collider EnemyCollider;

	public int m_ExpToGive;
	NavMeshAgent agent;
	Animator animator;
	AudioSource audiosource;
	public AudioClip enemyHitSound;
	public AudioClip enemyHitSoundTwo;
	public AudioClip enemyFootStep;
	public AudioClip enemyKickedSound;
	public AudioClip enemyDeathSound;
	[SerializeField] private Collider m_EnemySword = default;

	//float Speed = 1f;

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
	public bool firstHitSound = false;
	public bool secondHitSound = false;
	public bool hasPlayed = false;
	public float m_caughtDistance = 2.5f;
	public float m_chaseDistance = 8.5f;

	public float m_Healthpoint = 30f;
	public PlayerAttack m_SwordAttack;
	public GameObject m_BossDoorKey;
	public float m_DropChance = 0.5f;
	public SAMI_EnemyDamage m_Damage;

	// Start is called before the first frame update
	void Start()
	{
		agent = GetComponent<NavMeshAgent>();

		animator = GetComponent<Animator>();

		audiosource = GetComponent<AudioSource>();

		m_Damage = FindObjectOfType<SAMI_EnemyDamage>();

		m_PlayerStats = FindObjectOfType<SAMI_PlayerStats>();

		m_EnemySword.enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (m_Healthpoint <= 0)
		{
			if (isDead == false)
			{
				m_PlayerStats.AddExperience(m_ExpToGive);
				Instantiate(m_BossDoorKey, transform.position, transform.rotation);
			}

			StartCoroutine(EnemyDeath());
		}

		//if(Vector3.Distance(transform.position, otherEnemy.position) < 1.5f)
		//{
		//	transform.position = Vector3.MoveTowards(transform.position, otherEnemy.position, -1 * Speed * Time.deltaTime);
		//}

		if (Vector3.Distance(transform.position, Player.position) < m_caughtDistance && isHit == false && isKicked == false && inSafeHouse == false && isDead == false && attacking == false)
		{
			StartCoroutine(AttackingPlayer());
		}
		else if (Vector3.Distance(transform.position, Player.position) < m_chaseDistance && isHit == false && attacking == false && isKicked == false && inSafeHouse == false && isDead == false)
		{
			ChasePlayer();
		}
		else if (Vector3.Distance(transform.position, Waypoint.position) < 0.35f && isHit == false && attacking == false && isKicked == false && isDead == false)
		{
			EnemyIdle();
		}
		else if (Vector3.Distance(transform.position, Player.position) > m_chaseDistance && playerHasAttacked == false && inSafeHouse == false && attacking == false && isWaiting == false
				 && isKicked == false && isHit == false && isDead == false || inSafeHouse == true)
		{
			StartPatrolling();
		}
		else if (playerHasAttacked == true && isKicked == false && attacking == false && isHit == false && inSafeHouse == false && isDead == false)
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
		transform.LookAt(targetPosition);

		animator.SetBool("Attack", true);
		animator.SetBool("Walk", false);
		//animator.SetBool("Patrol", false);
		animator.SetBool("Idle", false);
		animator.SetBool("Kicked", false);
		animator.SetBool("isHit", false);

		yield return new WaitForSeconds(2.5f);
		attacking = false;
	}
	public void EnemyAttackColliderEnable()
	{
		if (isHit == false && isKicked == false)
		{
			m_EnemySword.enabled = true;
		}
	}

	public void EnemyAttackColliderDisable()
	{
		m_EnemySword.enabled = false;
	}

	public void ChasePlayer()
	{
		isWaiting = false;

		//Follow the player
		agent.destination = Player.position;
		agent.speed = 2f;

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

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Sword") && isHit == false)
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
		if (hasPlayed == false && firstHitSound == false && m_Healthpoint > 0)
		{

			audiosource.pitch = Random.Range(0.8f, 1f);
			audiosource.PlayOneShot(enemyHitSound, 0.55f);

			firstHitSound = true;
			secondHitSound = false;
		}
		else if (hasPlayed == false && secondHitSound == false && m_Healthpoint > 0)
		{

			audiosource.pitch = Random.Range(0.8f, 1f);
			audiosource.PlayOneShot(enemyHitSoundTwo, 0.55f);

			firstHitSound = false;
			secondHitSound = true;
		}

		hasPlayed = true;
		m_Healthpoint -= m_SwordAttack.m_DamageDone;
		attacking = false;
		playerHasAttacked = true;
		isHit = true;
		agent.speed = 0f;
		//animator.SetBool("Patrol", false);
		animator.SetBool("Kicked", false);
		animator.SetBool("Walk", false);
		animator.SetBool("isHit", true);
		animator.SetBool("Idle", false);
		animator.SetBool("Attack", false);
		yield return new WaitForSeconds(1.35f);
		hasPlayed = false;
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
		playerHasAttacked = false;
		inSafeHouse = true;
	}

	public void LeavingSafeHouse()
	{
		inSafeHouse = false;
	}

	public void EnemeyFootStep()
	{
		audiosource.volume = Random.Range(0.75f, 1f);
		audiosource.pitch = Random.Range(0.9f, 1f);

		audiosource.PlayOneShot(enemyFootStep);
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
		Vector3 targetPosition = new Vector3(lookAtWhenIdle.position.x, this.transform.position.y, lookAtWhenIdle.position.z);
		transform.LookAt(targetPosition);
	}

	public void EnemeyKickedSound()
	{
		if (hasPlayed == false && m_Healthpoint > 0)
		{
			audiosource.PlayOneShot(enemyKickedSound, 0.8f);


		}
	}

	#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, m_chaseDistance);
		Gizmos.DrawWireSphere(transform.position, m_caughtDistance);
	}
	#endif
}
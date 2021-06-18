using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Copy_Of_HalilinEnemy : MonoBehaviour
{
	public Transform Player = default;
	public Transform lookatwhenattack = default;

	public UnityEvent Trigger = default;
	
	NavMeshAgent agent;
	Animator animator;

	//[SerializeField] float decisionDelay = 3f;
	[SerializeField] Transform[] waypoints = default;
	int currentWaypoint = 0;

	//bool hitPlayed = false;
	public bool isCaught = false;
	public bool isHit = false;
	public  bool hasPlayed = false;
	public float m_caughtDistance = 3f;
	public float m_chaseDistance = 15f;
	public float m_stopChaseDistance = 25f;

	public float m_Healthpoint = 100f;

	// Start is called before the first frame update
	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Vector3.Distance(transform.position, Player.position) < m_caughtDistance && isHit == false)
		{
			AttackPlayer();
		}
		else if (Vector3.Distance(transform.position, Player.position) < m_chaseDistance && isHit == false)
		{
			
			ChasePlayer();
			
		}
		else if (Vector3.Distance(transform.position, Player.position) > m_stopChaseDistance && isHit == false)
		{
			StartPatrolling();
		}
	}

	void AttackPlayer()
	{
		agent.speed = 0f;
		transform.LookAt(lookatwhenattack);
		animator.SetBool("Attack", true);
		animator.SetBool("Chase", false);
		animator.SetBool("Patrol", false);
		animator.SetBool("Idle", false);
		animator.SetBool("isHit", false);
	}

	public void  ChasePlayer()
	{
		//Follow the player
		agent.destination = Player.position;
		agent.speed = 4f;
		animator.SetBool("Attack", false);
		animator.SetBool("Chase", true);
		animator.SetBool("Patrol", false);
		animator.SetBool("Idle", false);
		animator.SetBool("isHit", false);
		animator.SetBool("shieldStrafe", false);
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
		agent.speed = 2f;

		animator.SetBool("Attack", false);
		animator.SetBool("Chase", false);
		animator.SetBool("Patrol", true);
		animator.SetBool("Idle", false);
		animator.SetBool("isHit", false);
		animator.SetBool("shieldStrafe", false);
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Sword"))
		{
			StartCoroutine(GetHit());
		}
	}


	IEnumerator GetHit()
	{
		isHit = true;
		agent.speed = 0f;
		animator.SetBool("Attack", false);
		animator.SetBool("Chase", false);
		animator.SetBool("Patrol", false);
		animator.SetBool("Idle", false);
		animator.SetBool("isHit", true);
		animator.SetBool("shieldStrafe", false);
		yield return new WaitForSeconds(0.65f);
		isHit = false;
	}

	#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, m_chaseDistance);
		Gizmos.DrawWireSphere(transform.position, m_caughtDistance);
		Gizmos.DrawWireSphere(transform.position, m_stopChaseDistance);
	}
	#endif
}
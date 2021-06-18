using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Enemy : MonoBehaviour
{
	NavMeshAgent agent;

	public float patrolSpeed = 2f;
	public float chaseSpeed = 4f;

	

	public static Vector3 Point;

	[SerializeField] float decisionDelay = 3f;
	[SerializeField] Transform objectToChase = default;
	[SerializeField] Transform[] waypoints = default;
	int currentWaypoint = 0;
	[SerializeField]
	private float m_ChaseDistance = 10f;

	

	Animator animator;

	enum EnemyStates
	{
		Patrolling,
		Chasing
	}

	[SerializeField] EnemyStates currentState;

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		InvokeRepeating("SetDestination", 0.5f, decisionDelay);
		if (currentState == EnemyStates.Patrolling)
			agent.SetDestination(waypoints[currentWaypoint].position);

		animator = GetComponent<Animator>();

	}

	void Update()
	{
		if (Vector3.Distance(transform.position, objectToChase.position) > m_ChaseDistance)
		{
			currentState = EnemyStates.Patrolling;

			agent.speed = patrolSpeed;

			animator.SetBool("Patrol", true);
			animator.SetBool("Chase", false);



		}
		else
		{
			currentState = EnemyStates.Chasing;

		}
		if (currentState == EnemyStates.Patrolling)
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

		}
	}

	// void FixedUpdate()
	//{
	//	Vector3 direction = Point - transform.position;
	//	Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
	//	transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 1 * Time.time);
	//}

	void SetDestination()
	{
		if (currentState == EnemyStates.Chasing)
		{
			agent.SetDestination(objectToChase.position);
			animator.SetBool("Patrol", false);
			animator.SetBool("Chase", true);

			agent.speed = chaseSpeed;

		}



	}

	

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, m_ChaseDistance);
	}
}
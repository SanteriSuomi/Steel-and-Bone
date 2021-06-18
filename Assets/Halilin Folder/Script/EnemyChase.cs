using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
	
	public Transform transformToFollow;
	public Transform stopChasing;
	public AudioClip catchSound;
	NavMeshAgent agent;
	Animator animator;
	AudioSource audiosource;

	public bool safePlace = false;
	public bool chasePlayer = false;
	public bool hasPlayed = false;

	private void OnAnimatorIK(int layerIndex)
	{
		animator.SetLookAtPosition(new Vector3(0, 0, 0));
		animator.SetLookAtWeight(1, 0, 1);
	}


	// Start is called before the first frame update
	void Start()
	{
		agent = GetComponent<NavMeshAgent>();

		animator = GetComponent<Animator>();

		audiosource = GetComponent<AudioSource>();

	}

	// Update is called once per frame
	void Update()
	{
		if (chasePlayer == true)
		{
			ChasePlayer();
		}

		else if (safePlace == true)
		{
			agent.destination = stopChasing.position;
		}

		
	}

	void ChasePlayer()
	{
		//Follow the player
		agent.destination = transformToFollow.position;

		animator.SetBool("startChasing", true);

	}


	public void StartChasing()
	{
		chasePlayer = true;
		if (!hasPlayed)
		{
			audiosource.Play();
		}
		hasPlayed = true;
	}


	public void StopChasing()
	{
		chasePlayer = false;

		safePlace = true;

		audiosource.Stop();

		animator.SetBool("stopChasing", true);
	}

	

}

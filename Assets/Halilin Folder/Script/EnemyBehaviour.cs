using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
	Animator animator;
	AudioSource audiosource;

	public AudioClip hitSound;

	bool hitPlayed = false;


    // Start is called before the first frame update
    void Start()
    {
		animator = GetComponent<Animator>();

		audiosource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


	 void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Sword"))
		{
			GetHit();
		}
		
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Sword"))
		{
			Idling();
		}
	}

	void GetHit()
	{
		
		if (hitPlayed == false)
		{
			audiosource.PlayOneShot(hitSound);
			hitPlayed = true;
			animator.SetBool("isHit", true);
		}
	}

	void Idling()
	{
		animator.SetBool("Idle", true);
		animator.SetBool("isHit", false);
		hitPlayed = true;
	}

}

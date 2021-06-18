using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_ShopKeeper : MonoBehaviour
{
	Animator animator;
	AudioSource audiosource;
	public AudioClip Hello;
	public AudioClip Bye;
	public Transform player;

	public bool talking = false;
	public bool inShop = false;

	public float inShopDistance = 4f;
	public float outOfShopDistance = 6f;


    // Start is called before the first frame update
    void Start()
    {
		audiosource = GetComponent<AudioSource>();

		animator = GetComponent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {
		if (Vector3.Distance(transform.position, player.position) < inShopDistance && talking == false && inShop == false)
		{
			StartCoroutine(EnteringShop());
		}
		else if (Vector3.Distance(transform.position, player.position) > outOfShopDistance && talking == false && inShop == true)
		{
			StartCoroutine(LeavingShop());
		}
    }


	

	IEnumerator EnteringShop()
	{
		inShop = true;
		talking = true;
		audiosource.PlayOneShot(Hello);
		animator.SetBool("Talk", true);
		animator.SetBool("Bye", false);
		animator.SetBool("Idle", false);
		yield return new WaitForSeconds(2.5f);
		animator.SetBool("Idle", true);
		animator.SetBool("Talk", false);
		animator.SetBool("Bye", false);
		talking = false;
	}

	IEnumerator LeavingShop()
	{
		inShop = false;
		talking = true;
		audiosource.PlayOneShot(Bye);
		animator.SetBool("Talk", false);
		animator.SetBool("Bye", true);
		animator.SetBool("Idle", false);
		yield return new WaitForSeconds(2.5f);
		animator.SetBool("Idle", true);
		animator.SetBool("Talk", false);
		animator.SetBool("Bye", false);
		talking = false;
		
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, inShopDistance);
		Gizmos.DrawWireSphere(transform.position, outOfShopDistance);
	}
}

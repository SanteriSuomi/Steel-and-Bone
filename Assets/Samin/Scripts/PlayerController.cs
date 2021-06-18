using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] public float m_speed;
	[SerializeField] private Rigidbody m_rigid;





	void Start()
    {
		m_rigid = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
		// RandomPosition
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (transform.position.y <= 1.05f)
			{
				GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * 1000);
				//GetComponent<Rigidbody>().AddForce(Vector3.up * 1000); //jumping
			}
		}




		//Player Move and Speed
		float Horizontal = Input.GetAxis("Horizontal");
		float Vertical = Input.GetAxis("Vertical");


		Vector3 move = new Vector3(Horizontal, 0.0f, Vertical);
		
		m_rigid.AddForce(move * m_speed);
	}
}

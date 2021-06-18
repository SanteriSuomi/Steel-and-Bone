using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAutoRotate : MonoBehaviour
{
	public float m_speed;







	void Start()
    {
		
    }

    
    void Update()
    {
		transform.Rotate(new Vector3(0f, m_speed * 50f, 0f) * Time.deltaTime);
		

	}
}

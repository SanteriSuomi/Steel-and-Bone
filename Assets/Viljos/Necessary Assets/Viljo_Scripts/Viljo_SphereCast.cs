using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Viljo_SphereCast : MonoBehaviour
{
	[SerializeField] private float m_rayMeter;
	[SerializeField] private GameObject m_gameObject;
	[SerializeField] GameObject m_mainCamera;
	[SerializeField] bool m_carrying;
	[SerializeField] GameObject m_carriedObject;
	[SerializeField] private float m_distance = 1;
	[SerializeField] private float m_smooth = 1;
	[SerializeField] private Button[] m_button;
	[SerializeField] private float m_throwForce = 2000;
	Collider m_Collider;


	void Start()
	{
		m_mainCamera = GameObject.FindWithTag("MainCamera");        // Find GameObject tagged MainCamera in Start. 
		m_Collider = GetComponent<Collider>();                      // Find gameObject's collider in Start.

		m_gameObject = this.gameObject;                             // Find gameObject in Start which has this script attached to it.
		m_rayMeter = 2f;                                            // Ray's distance to reach the pickable gameObject. 

	}


	void Update()
	{
		if (m_carrying)                                             // Methods are functionable if boolean value of m_carrying is true. 
		{
			Carry(m_carriedObject);
			CheckDrop();
			ThrowObject();

		}
		else
		{
			PickUp();

		}
	}
	void Carry(GameObject o)                                            // Makes carrying of the gameObject possible.
	{

		o.transform.position = Vector3.Lerp(o.transform.position, m_mainCamera.transform.position + m_mainCamera.transform.forward * m_distance, Time.deltaTime * m_smooth);
	}


	void ThrowObject()
	{
		if (Input.GetMouseButtonDown(0))                                    // By pressing left mouse button the object is thrown at the direction of the camera with disabled collider and gravity added. 
		{

			m_carrying = false;
			m_carriedObject.gameObject.GetComponent<Rigidbody>().isKinematic = false;
			m_carriedObject.GetComponent<Rigidbody>().AddForce(m_mainCamera.transform.forward * m_throwForce);                      // Adds force to the object when it's thrown.
			m_Collider.enabled = true;
			m_gameObject.gameObject.GetComponent<Rigidbody>().useGravity = true;

		}
	}

	void PickUp()
	{
		if (Input.GetKeyDown(KeyCode.E))                                    // By pressing E a ray is shot and gameObject lifts off the ground and sets in middle of the camera view with collider and gravity disabled.
		{
			int x = Screen.width / 2;
			int y = Screen.height / 2;

			Ray ray = m_mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y));
			RaycastHit hit;                                                                                         // Returns a ray going from the middle of the camera through a screen point.

			if ((Physics.SphereCast(origin: transform.position, radius: transform.lossyScale.x / 2, direction: transform.right, out hit, m_rayMeter)) && hit.transform.gameObject == m_gameObject)            // Object transform after casted ray detects collision with gameObject.
			{

				m_Collider.enabled = false;
				m_gameObject.gameObject.GetComponent<Rigidbody>().useGravity = false;


				if (m_gameObject != null)                                                           // Compares if two objects refer to a different object than m_gameObject.
				{

					m_carrying = true;
					m_carriedObject = m_gameObject;
					m_gameObject.gameObject.GetComponent<Rigidbody>().isKinematic = true;

				}

			}

		}
	}

	void CheckDrop()
	{
		if (Input.GetKeyDown(KeyCode.E))                                        // By pressing E gameObject is released with it's collider and gravity disabled.
		{
			m_carrying = false;
			m_carriedObject.gameObject.GetComponent<Rigidbody>().isKinematic = false;
			m_Collider.enabled = true;
			m_gameObject.gameObject.GetComponent<Rigidbody>().useGravity = true;

		}




		/*void MusicPlayBox()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                int x = Screen.width / 2;
                int y = Screen.height / 2;

                Ray ray = m_mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, m_rayMeter = 2f));
                {
                    m_button.gameObject.SetActive(True);



                        print("TOIMIII");
                }


            }
        }*/
	}
}


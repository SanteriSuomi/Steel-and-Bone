using UnityEngine;

public class Viljo_PickUpObject : MonoBehaviour
{
	[SerializeField] private GameObject m_player = default;
	[SerializeField] private GameObject m_object = default;
	[SerializeField] private GameObject m_tempParent = default;
	[SerializeField] private Transform m_guide = default;
	private float m_pickDistance = 2;
	private bool m_isPicked = false;
	//int m_xDirection = 0;
	//int m_zDirection = 0;
	//private float m_xDist;
	//private float m_yPos;
	//private float m_zDist;

	void Start()
	{
		m_object.GetComponent<Rigidbody>().useGravity = true;     // Find GameObject's Rigidbody and give it a value of true.
	}

	void Update()
	{
		PickUp();               // Method PickUp() is called.
								//Drop();
	}

	void PickUp()
	{
		if (Input.GetKey(KeyCode.Mouse0))           // Object lifts off the ground.
		{
			if (m_isPicked)
			{
				m_isPicked = false;
			}
			else if (Vector3.Distance(gameObject.transform.position, m_player.transform.position) < m_pickDistance)
			{
				m_isPicked = true;
				m_object.GetComponent<Rigidbody>().useGravity = false;
				m_object.GetComponent<Rigidbody>().isKinematic = true;
				m_object.GetComponent<Rigidbody>().detectCollisions = true;
				m_object.transform.position = m_guide.transform.position;
				m_object.transform.rotation = m_guide.transform.rotation;
				m_object.transform.parent = m_tempParent.transform;

			}

			/*if (m_isPicked == false)
			{
				m_object.GetComponent<Rigidbody>().useGravity = true;
				m_object.GetComponent<Rigidbody>().isKinematic = false;
				m_object.transform.parent = null;
				m_object.transform.position = m_guide.transform.position;
			}*/
		}
	}

	/*void OnMouseDown()      // Holding Mouse0 down lifts GameObject.
	{
			m_item.GetComponent<Rigidbody>().useGravity = false;
			m_item.GetComponent<Rigidbody>().isKinematic = true;
			m_item.GetComponent<Rigidbody>().detectCollisions = true;
			m_item.transform.position = m_guide.transform.position;
			m_item.transform.rotation = m_guide.transform.rotation;
			m_item.transform.parent = m_tempParent.transform;
	}

	private void OnMouseUp()		// Releasing Mouse0 drops GameObject.
	{
		m_item.GetComponent<Rigidbody>().useGravity = true;
		m_item.GetComponent<Rigidbody>().isKinematic = false;
		m_item.transform.parent = null;
		m_item.transform.position = m_guide.transform.position;
	}*/

	/*void Drop()
	{
		if (Input.GetKey(KeyCode.Mouse1))
		{
			m_isPicked = false;
			m_object.GetComponent<Rigidbody>().useGravity = true;
			m_object.GetComponent<Rigidbody>().isKinematic = false;
			m_object.transform.parent = null;
			m_object.transform.position = m_guide.transform.position;
		}
	}*/
}
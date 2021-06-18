using UnityEngine;

public class Viljo_ItemPickUp : MonoBehaviour
{
	[SerializeField] private GameObject m_item = default;
	[SerializeField] private GameObject m_tempParent = default;
	[SerializeField] private Transform m_guide = default;

	//private float m_throwForce = 600;
	//private Vector3 m_objectPosition;
	//private float m_distance;

	private void Start()
	{
		m_item.GetComponent<Rigidbody>().useGravity = true;     // Find GameObject's Rigidbody and give it a value of true.
	}

	private void Update()
	{
		PickUp();
		Drop();
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

	void PickUp()
	{
		if (Input.GetKey(KeyCode.E))
		{
			m_item.GetComponent<Rigidbody>().useGravity = false;
			m_item.GetComponent<Rigidbody>().isKinematic = true;
			m_item.GetComponent<Rigidbody>().detectCollisions = true;
			m_item.transform.position = m_guide.transform.position;
			m_item.transform.rotation = m_guide.transform.rotation;
			m_item.transform.parent = m_tempParent.transform;
		}
	}

	void Drop()
	{
		if (Input.GetKey(KeyCode.E))
		{
			m_item.GetComponent<Rigidbody>().useGravity = true;
			m_item.GetComponent<Rigidbody>().isKinematic = false;
			m_item.transform.parent = null;
			m_item.transform.position = m_guide.transform.position;
		}
	}
}

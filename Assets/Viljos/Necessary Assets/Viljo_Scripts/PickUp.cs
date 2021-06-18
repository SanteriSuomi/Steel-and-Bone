using UnityEngine;

public class PickUp : MonoBehaviour
{
	public Transform m_thedest;
	//float m_distanceToObject = 3f;
	public float m_throwForce = 10f;
	public Rigidbody rb;
	public Transform m_cam;
	//public GameObject m_CurrentText;
	public static bool m_Image = false;
	public LayerMask m_LayerMask;
	//public GameObject m_PickUpText;
	//public GameObject m_CurrentText;
	//public float sphereradius = 2f;
	//[SerializeField] float distance = 0.2f;
	//[SerializeField] float speed = 0.2f;
	private Transform parent;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		parent = GameObject.Find("PickUpDestination").transform;
	}

	private void FixedUpdate()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			rb.AddForce(m_cam.forward * m_throwForce);
			rb.useGravity = true;
			OnMouseUp();
		}
	}

	//private void Update()
	//{
	//	if (Physics.Raycast(m_thedest.transform.position, m_thedest.transform.forward, 0.5f, m_LayerMask))
	//	{
	//		//m_PickUpText.SetActive(true);
	//	}
	//	else
	//	{
	//		//m_PickUpText.SetActive(false);
	//	}
	//}

	// Start is called before the first frame update
	private void OnMouseDown()
	{
		if (Physics.Raycast(m_thedest.transform.position, m_thedest.transform.forward, 0.5f, m_LayerMask))
		{
			//GetComponent<BoxCollider>().enabled = false;
			rb.useGravity = false;
			//m_PickUpText.SetActive(false);
			transform.position = m_thedest.position;
			transform.parent = parent;
			//ObjectiveCounter.scoreValue += 1;

			if (Input.GetMouseButtonUp(1))
			{
				rb.AddForce(m_thedest.forward * m_throwForce);
			}

			//if (Input.GetKeyDown(KeyCode.E))
			//{
			//	m_CurrentText.SetActive(true);
			//}
			//m_CurrentText.SetActive(false);
		}
	}

	private void OnCollisionEnter(Collision _)
	{
		OnMouseUp();
	}

	private void OnMouseUp()
	{
		transform.parent = null;
		rb.useGravity = true;
		//GetComponent<BoxCollider>().enabled = true;
	}

	//void Zoom()
	//{
	//	if (distance > 5)
	//	{
	//		distance = 5;
	//	}

	//	if (distance < 2)
	//	{
	//		distance = 2;
	//	}

	//	if (Input.GetAxis("Mouse ScrollWheel") > 0 && distance <= 5)
	//	{
	//		distance += speed;
	//	}

	//	if (Input.GetAxis("Mouse ScrollWheel") < 0 && distance >= 2)
	//	{
	//		distance -= speed;
	//	}
	//}

	#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(m_thedest.transform.position, 2f);
	}
	#endif
}
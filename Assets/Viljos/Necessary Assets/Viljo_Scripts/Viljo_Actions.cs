using System.Collections;
using UnityEngine;
using UnityEngine.AI;

#pragma warning disable
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Viljo_Actions : MonoBehaviour
{
	public static bool CanDoDamage { get; private set; }
	public bool IsCarrying { get; set; }

	private GameObject carriedObject;
	private Rigidbody carriedRigidbody;
	private Collider carriedCollider;
	//private WaitForSeconds disableCanDoDamageWFS;

	private const float m_distance = 0.625f;
	private const float m_smooth = 100;
	private const float m_throwForce = 900;
	private const float m_rayMeter = 3;
	private const float outOfBoundsCheckDistance = 5;
	private const float outOfBoundsMinDistance = 0.75f;
	private const float outOfBoundsTeleportYOffset = 0.75f;

	private static WaitForSeconds raycastEnableWFS;
	private static Camera mainCamera;
	private static bool hasCameraReference;
	private static bool isCastingPickupRay;

	private void OnEnable()
	{
		isCastingPickupRay = false;
		hasCameraReference = false;
		CanDoDamage = false;
		//disableCanDoDamageWFS = new WaitForSeconds(1);
	}

	private void Start()
	{
		raycastEnableWFS = InteractableComponents.Instance.ActionsRaycastEnableWFS;
		if (!hasCameraReference)
		{
			hasCameraReference = true;
			mainCamera = Camera.main;
		}
	}

	private void LateUpdate()
	{
		if (IsCarrying) // Methods are functionable if boolean value of m_carrying is true. 
		{
			CheckDrop();
			ThrowObject();
			Carry(carriedObject);
		}
		else
		{
			PickUp();
		}
	}

	private void Carry(GameObject obj) // Makes carrying of the gameObject possible. Also m_smooth is value which defines the speed of GameObject when following camera view.
	{
		obj.transform.position = Vector3.Lerp(obj.transform.position,
			mainCamera.transform.position + mainCamera.transform.forward * m_distance, Time.deltaTime * m_smooth);
	}

	private void CheckDrop()
	{
		if (Input.GetKeyDown(KeyCode.E) && IsCarrying) // By pressing E gameObject is released with it's collider and gravity disabled.
		{
			IsCarrying = false;
			PlayerAttack.IsCarryingThrowable = false;

			carriedRigidbody.isKinematic = false;
			carriedCollider.enabled = true;
			carriedRigidbody.useGravity = true;

			carriedObject.layer = 0; // Default layer

			Invoke(nameof(CheckOutOfBounds), 1);
		}
	}

	private void CheckOutOfBounds()
	{
		NavMesh.SamplePosition(transform.position, out NavMeshHit hit, outOfBoundsCheckDistance, NavMesh.AllAreas);
		if (hit.hit && hit.distance >= outOfBoundsMinDistance)
		{
			transform.position = hit.position + new Vector3(0, outOfBoundsTeleportYOffset, 0);
		}
	}

	private void ThrowObject()
	{
		if (Input.GetMouseButtonDown(0) && IsCarrying) // By pressing left mouse button the object is thrown at the direction of the camera with disabled collider and gravity added. 
		{
			CanDoDamage = true;
			//StartCoroutine(CheckDisableCanDoDamage(carriedRigidbody));
			Invoke(nameof(DisableCanDoDamage), 3);
			IsCarrying = false;
			PlayerAttack.IsCarryingThrowable = false;

			carriedRigidbody.isKinematic = false;
			carriedCollider.enabled = true;
			carriedRigidbody.useGravity = true;
			carriedRigidbody.AddForce(mainCamera.transform.forward * m_throwForce); // Adds force to the object when it's thrown.

			carriedObject.layer = 0; // Default layer
		}
	}

	//private IEnumerator CheckDisableCanDoDamage(Rigidbody rb)
	//{
	//	while (rb.velocity.sqrMagnitude > 0.01f)
	//	{
	//		yield return null;
	//	}

	//	yield return disableCanDoDamageWFS;
	//	CanDoDamage = false;
	//}

	private void DisableCanDoDamage()
	{
		CanDoDamage = false;
	}

	private void PickUp()
	{
		if (Input.GetKeyDown(KeyCode.E)) // By pressing E a ray is shot and gameObject lifts off the ground and sets in middle of the camera view with collider and gravity disabled.
		{
			// Returns a ray going from the middle of the camera through a screen point.
			(bool didHit, RaycastHit hitInfo) data = (false, new RaycastHit());
			if (!isCastingPickupRay)
			{
				StartCoroutine(RaycastEnableAgain());
				data = GetRaycast(m_rayMeter);
			}

			if (data.didHit && data.hitInfo.collider.CompareTag("Throwable"))
			{
				carriedObject = data.hitInfo.transform.gameObject;
				carriedRigidbody = data.hitInfo.transform.GetComponent<Rigidbody>();
				carriedCollider = data.hitInfo.transform.GetComponent<Collider>();

				carriedRigidbody.isKinematic = true;
				carriedCollider.enabled = false;
				carriedRigidbody.useGravity = false;

				carriedObject.layer = 15; // Throwables layer

				IsCarrying = true;
				PlayerAttack.IsCarryingThrowable = true;
			}
		}
	}

	private static (bool, RaycastHit) GetRaycast(float length)
	{
		int x = Screen.width / 2;
		int y = Screen.height / 2;
		Ray ray = mainCamera.ScreenPointToRay(new Vector3(x, y));
		bool didHit = Physics.Raycast(ray, out RaycastHit hit, length);
		return (didHit, hit);
	}

	private IEnumerator RaycastEnableAgain()
	{
		SetIsCasting(true);
		yield return raycastEnableWFS;
		SetIsCasting(false);
	}

	private static void SetIsCasting(bool value)
	{
		isCastingPickupRay = value;
	}
}
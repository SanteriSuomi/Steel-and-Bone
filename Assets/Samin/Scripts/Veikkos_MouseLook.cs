using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Veikkos_MouseLook : MonoBehaviour
{//Add this to Camera

	[SerializeField] private string mouseXInputName = default, mouseYInputName = default;
	[SerializeField] private float m_sensib = default;
	//[SerializeField] public Rigidbody m_rigidbody;
	[SerializeField] private Transform m_pBody = default;
	//[SerializeField] Quaternion m_originalRotation;
	private float xAxisClamp;

	void Start()
	{
		// Make the rigidbody follow camerarotation
		//if (m_rigidbody)
			//m_rigidbody.freezeRotation = true;
		//m_originalRotation = transform.localRotation;
	}

	private void Awake()
    {
		xAxisClamp = 0.0f;
    }

    
    void Update()
    {
		CameraRotation();
    }

	private void CameraRotation()
	{
		float mouseX = Input.GetAxis(mouseXInputName) * m_sensib * Time.deltaTime;
		float mouseY = Input.GetAxis(mouseYInputName) * m_sensib * Time.deltaTime;

		xAxisClamp += mouseY;
		
			if(xAxisClamp > 90.0f)
		{
			xAxisClamp = 90.0f;
				mouseY = 0.0f;
			ClampAxisRotationToValue(270.0f);
		}

		else if (xAxisClamp < -90.0f)
		{
			xAxisClamp = -90.0f;
			mouseY = 0.0f;
			ClampAxisRotationToValue(90.0f);
		}

		transform.Rotate(Vector3.left * mouseY);
		m_pBody.Rotate(Vector3.up * mouseX);
	}

	private void ClampAxisRotationToValue(float value)
	{
		Vector3 eulerRotation = transform.eulerAngles;
		eulerRotation.x = value;
		transform.eulerAngles = eulerRotation;
	}
}

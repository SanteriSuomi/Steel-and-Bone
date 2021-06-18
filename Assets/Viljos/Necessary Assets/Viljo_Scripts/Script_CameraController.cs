using UnityEngine;

namespace TheDiamondCollector
{
	public class Script_CameraController : MonoBehaviour
	{
		[SerializeField] private string m_mouseXInputName = default, m_mouseYInputName = default;
		[SerializeField] private float m_mouseSensitivity = 80;
		[SerializeField] Transform m_playerBody = default;

		private float m_xAxisClamp;

		public void Awake()
		{
			LockCursor();
			m_xAxisClamp = 0.0f;
		}

		private void LockCursor()
		{
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = false;
		}

		private void Update()
		{
			CameraRotation();
		}

		private void CameraRotation()
		{
			float mouseX = Input.GetAxis(m_mouseXInputName) * m_mouseSensitivity * Time.deltaTime;
			float mouseY = Input.GetAxis(m_mouseYInputName) * m_mouseSensitivity * Time.deltaTime;

			m_xAxisClamp += mouseY;

			if (m_xAxisClamp > 90.0f)
			{
				m_xAxisClamp = 90.0f;
				mouseY = 0.0f;
				ClampXAxisRotationToValue(270.0f);
			}
			else if (m_xAxisClamp < -90.0)
			{
				m_xAxisClamp = -90.0f;
				mouseY = 0.0f;
				ClampXAxisRotationToValue(90.0f);
			}

			transform.Rotate(Vector3.left * mouseY);
			m_playerBody.Rotate(Vector3.up * mouseX);
		}

		private void ClampXAxisRotationToValue(float value)
		{
			Vector3 eulerRotation = transform.eulerAngles;
			eulerRotation.x = value;
			transform.eulerAngles = eulerRotation;
		}
	}
}
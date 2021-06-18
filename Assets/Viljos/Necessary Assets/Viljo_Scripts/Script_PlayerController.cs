using System.Collections;
using UnityEngine;

namespace TheDiamondCollector
{
	public class Script_PlayerController : MonoBehaviour
	{
		//private Rigidbody m_rb;
		[SerializeField] private string m_horizontalInputName = default;
		[SerializeField] private string m_verticalInputName = default;
		[SerializeField] private float m_movementSpeed = 8;
		[SerializeField] private KeyCode m_jumpKey = default;
		[SerializeField] private float m_jumpMultiplier = 10;
		[SerializeField] AnimationCurve m_jumpFallOff = default;
		private bool m_isJumping;
		private CharacterController m_characterController;
		//public Text m_countText;
		//private int m_count;
		//public AudioSource m_AudioSource;
		//[SerializeField] private GameObject m_player;
		//[SerializeField] private Transform m_teleport1;
		//[SerializeField] private Transform m_teleport2;
		//[SerializeField] private Transform m_teleport3;

		private void Awake()
		{
			m_characterController = GetComponent<CharacterController>();
		}

		private void Start()
		{
			//m_rb = GetComponent<Rigidbody>();
			//m_count = 0;
			//SetCountText();
		}

		void Update()
		{
			PlayerMovement();
			//CollectedAll();
		}

		private void PlayerMovement()
		{
			float horizontalInput = Input.GetAxis(m_horizontalInputName) * m_movementSpeed;
			float verticalInput = Input.GetAxis(m_verticalInputName) * m_movementSpeed;

			Vector3 forwardMovement = transform.forward * verticalInput;
			Vector3 rightMovement = transform.right * horizontalInput;

			m_characterController.SimpleMove(forwardMovement + rightMovement);
			JumpInput();
		}

		private void JumpInput()
		{
			if (Input.GetKey(m_jumpKey) && !m_isJumping)
			{
				m_isJumping = true;
				StartCoroutine(JumpEvent());
			}
		}

		private IEnumerator JumpEvent()
		{
			float timeInAir = 0.0f;
			do
			{
				float jumpPower = m_jumpFallOff.Evaluate(timeInAir);
				m_characterController.Move(Vector3.up * jumpPower * m_jumpMultiplier * Time.deltaTime);
				timeInAir += Time.deltaTime;
				yield return null;
			} while (!m_characterController.isGrounded && m_characterController.collisionFlags != CollisionFlags.Above);

			m_isJumping = false;
		}

		/*private void OnTriggerEnter(Collider collider)
		{
			if (collider.gameObject.CompareTag("Collectable"))
			{
				collider.gameObject.SetActive(false);
				m_count = m_count + 1;
				m_AudioSource.Play();
				SetCountText();
			}

			TeleportOne();

			TeleportTwo();

			TeleportThree();
		}

		void SetCountText()
		{
			m_countText.text = "COLLECTED: " + m_count.ToString();
		}

		void CollectedAll()
		{
			if (m_count >= 20)
			{
				SceneManager.LoadScene("SCE_Menu_Completed_Game");
				Cursor.visible = true;
			}
		}

		void TeleportOne()
		{
			if (m_count == 4)
			{
				m_player.transform.position = m_teleport1.position;
			}

		}

		void TeleportTwo()
		{
			if (m_count == 8)
			{
				m_player.transform.position = m_teleport2.position;
			}
		}

		void TeleportThree()
		{
			if (m_count == 15)
			{
				m_player.transform.position = m_teleport3.position;
			}
		}*/
	}
}
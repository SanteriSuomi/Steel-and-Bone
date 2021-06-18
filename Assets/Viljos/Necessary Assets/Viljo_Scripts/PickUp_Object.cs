using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_Object : MonoBehaviour
{
	public GameObject m_player;
	private bool m_isPicked = false;

	private float m_pickDistance = 2;
	private float m_riseAmount = 1.2f;
	private float m_yPos;
	float m_xDist = 0;
	float m_zDist = 0;
	int m_xDirection = 0;
	int m_zDirection = 0;

	private void Start()
	{

	}


	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			if (m_isPicked)
			{
				m_isPicked = false;


			}


			else if (m_isPicked == false && Vector3.Distance(gameObject.transform.position, m_player.transform.position) < m_pickDistance)
			{
				Debug.Log("pickataan nyt!!!!");


				m_isPicked = true;
				var objPosition = gameObject.transform.position;
				var playerPosition = m_player.transform.position;


				if (objPosition.x > playerPosition.x)
				{
					m_xDirection = 1;
				}
				else
				{
					m_xDirection = -1;
				}
				if (objPosition.z > playerPosition.z)
				{
					m_zDirection = 1;
				}
				else
				{
					m_zDirection = -1;
				}




				m_yPos = objPosition.y + m_riseAmount;
				m_xDist = Mathf.Abs(objPosition.x - playerPosition.x);
				m_zDist = Mathf.Abs(objPosition.z - playerPosition.z);

			}

		}
		if (m_isPicked)
		{
			var playerPosition = m_player.transform.position;

			var newPos = new Vector3(m_xDirection * m_xDist + playerPosition.x, m_yPos, m_zDirection * m_zDist + playerPosition.z);
			gameObject.transform.position = newPos;
			//transform.Rotate(new Vector3(50, 50, 50));
		}
	}
}


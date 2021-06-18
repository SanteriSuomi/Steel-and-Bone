using System.Collections;
using UnityEngine;

public class BossAoeSpawner : MonoBehaviour
{
	public GameObject m_FireBall;
	public int xPos;
	public int zPos;
	public int m_FireBallCount;
	public GameObject AoeSpawner;

	public void StartMeteorSwarm()
	{
		StartCoroutine(FireDrop());
	}

	private IEnumerator FireDrop()
	{
		while (m_FireBallCount < 3)
		{
			xPos = Random.Range(-10, -30);
			zPos = Random.Range(76, 93);
			Instantiate(m_FireBall, new Vector3(xPos, 0.3f, zPos), Quaternion.identity);
			yield return new WaitForSeconds(0.1f);
			m_FireBallCount += 1;
			yield return new WaitForSeconds(2f);
			m_FireBallCount = 0;
		}
	}
}
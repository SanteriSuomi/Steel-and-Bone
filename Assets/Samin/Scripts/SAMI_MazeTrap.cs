using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAMI_MazeTrap : MonoBehaviour
{
    public Transform m_FlameThrowerMuzzle;
    public Transform m_FlameThrowerMuzzle1;
    public Transform m_FlameThrowerMuzzle2;
    public Transform m_FlameThrowerMuzzle3;
    public Transform m_FlameThrowerMuzzle4;
    public GameObject m_FireBall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Instantiate(m_FireBall, m_FlameThrowerMuzzle.position, Quaternion.identity);
            Instantiate(m_FireBall, m_FlameThrowerMuzzle1.position, Quaternion.identity);
            Instantiate(m_FireBall, m_FlameThrowerMuzzle2.position, Quaternion.identity); 
            Instantiate(m_FireBall, m_FlameThrowerMuzzle3.position, Quaternion.identity);
            Instantiate(m_FireBall, m_FlameThrowerMuzzle4.position, Quaternion.identity);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAMI_InstantiateMiniBossAoe : MonoBehaviour
{
    public Transform m_FlameThrowerMuzzle;
    public Transform m_FlameThrowerMuzzle1;
  
    public GameObject m_FireBall;

    // Update is called once per frame
    public void ExecuteAoe()
    {
        Instantiate(m_FireBall, m_FlameThrowerMuzzle.position, Quaternion.identity);
        Instantiate(m_FireBall, m_FlameThrowerMuzzle1.position, Quaternion.identity);
       
    }
}

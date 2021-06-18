using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbobber : MonoBehaviour
{
    private float m_timer = 0.0f;
    public float m_bobbingSpeed = 0.18f;
    public float m_bobbingAmount = 0.2f;
    public float m_midpoint = 2.0f;

    void Update()
    {
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            m_timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(m_timer);
            m_timer = m_timer + m_bobbingSpeed;
            if (m_timer > Mathf.PI * 2)
            {
                m_timer = m_timer - (Mathf.PI * 2);
            }
        }

        Vector3 v3T = transform.localPosition;
        if (waveslice != 0)
        {
            float translateChange = waveslice * m_bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            v3T.y = m_midpoint + translateChange;
        }
        else
        {
            v3T.y = m_midpoint;
        }
        transform.localPosition = v3T;

    }
}
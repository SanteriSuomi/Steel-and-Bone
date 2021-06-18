using System.Collections;
using UnityEngine;

public class Veikkos_Headbobber : MonoBehaviour
{
    private float m_timer = 0.0f;
    public float m_bobbingSpeed = 0.18f;
    public float m_bobbingAmount = 0.2f;
    public float m_midpoint = 1.5f;
    private WaitForFixedUpdate bobbingWFS;

    private void Awake()
    {
        bobbingWFS = new WaitForFixedUpdate();
    }

    private void OnEnable()
    {
        StartCoroutine(BobbingCoroutine());
    }

    private IEnumerator BobbingCoroutine()
    {
        while (enabled)
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
                m_timer += m_bobbingSpeed;
                float doublePI = Mathf.PI * 2;
                if (m_timer > doublePI)
                {
                    m_timer -= doublePI;
                }
            }

            Vector3 v3T = transform.localPosition;
            if (waveslice != 0)
            {
                float translateChange = waveslice * m_bobbingAmount;
                float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
                translateChange = totalAxes * translateChange;

                v3T.y = m_midpoint + (translateChange / 2f);
            }
            else
            {
                v3T.y = m_midpoint;
            }

            transform.localPosition = v3T;
            yield return bobbingWFS;
        }
    }

    private void OnDisable()
    {
        StopCoroutine(nameof(BobbingCoroutine));
    }
}
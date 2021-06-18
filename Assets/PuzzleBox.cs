using UnityEngine;

public class PuzzleBox : MonoBehaviour
{
    private Rigidbody m_BoxTransform;
    [SerializeField]
    private float m_Force = 12.5f;
    private GameObject m_Player;

    private void Awake()
    {
        m_Player = GameObject.Find("Player");
        m_BoxTransform = GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider other)
    {
        bool isSword = other.gameObject.CompareTag("Sword");

        if (other.gameObject.CompareTag("Foot")
            || (isSword && PlayerAttack.IsAttacking))
        {
            if (isSword)
            {
                m_BoxTransform.AddForce(m_Player.transform.forward * m_Force * 2);
            }
            else
            {
                m_BoxTransform.AddForce(m_Player.transform.forward * m_Force);
            }
        }
    }
}
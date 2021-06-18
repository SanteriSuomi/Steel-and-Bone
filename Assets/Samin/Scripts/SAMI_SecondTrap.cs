using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAMI_SecondTrap : MonoBehaviour
{
    float m_Speed = 90f;
    Rigidbody m_Rgbd;
    public Transform m_FireballTargetSecond;
    private AudioSource audioSource;



    private void Start()
    {
        m_Rgbd = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        m_FireballTargetSecond = GameObject.Find("SecondFireBallTarget").transform;
        m_Rgbd.AddRelativeForce((m_FireballTargetSecond.position - transform.position) * m_Speed);
        audioSource.Play();
        Invoke("SelfDestruct", 4f);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

    }


    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}

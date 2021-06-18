using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSS_SingleProjectile : MonoBehaviour
{
    public float m_Speed = 20f;
    public Rigidbody m_RB;
	public float m_Radius = 1f;
	public float m_ExplosionForce = 20f;
	public float m_Upwards = 0f;
    private void Start()
    {
		Vector3 explosionForce = transform.position;
		m_RB.velocity = transform.forward * m_Speed;
		m_RB.AddExplosionForce(m_ExplosionForce, explosionForce, m_Radius);
		Destroy(gameObject, 10);
		
    }
    


}

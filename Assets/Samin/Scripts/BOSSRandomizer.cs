using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSSRandomizer : MonoBehaviour
{
	public BossRanged m_BossRanged;
	public int m_Int;
    // Start is called before the first frame update
	public void Randomizer()
	{
		
		m_Int = Random.Range(1, 20);
		if(m_Int == 10)
		{
			m_BossRanged.m_PerformSpin = true;
		}
		
	} 
}

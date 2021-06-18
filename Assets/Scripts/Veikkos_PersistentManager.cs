using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Veikkos_PersistentManager : MonoBehaviour
{
    public static Veikkos_PersistentManager Instance
	{
		get; private set;
	}
	public int m_ValueInt;
	public float m_ValueFloat;

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
    
}

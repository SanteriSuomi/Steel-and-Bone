﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		StartCoroutine(Destroy());
    }

	IEnumerator Destroy()
	{
		yield return new WaitForSeconds(10f);
		Destroy(gameObject);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}

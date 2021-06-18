using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject EnemyOne;
    public GameObject EnemyTwo;
    public GameObject EnemyThree;

	private void Start()
	{
		Invoke(nameof(SpawnEnemies), 2);
	}
	public void SpawnEnemies()
    {
        StartCoroutine(EnemySpawning());
    }

    IEnumerator EnemySpawning()
    {
        EnemyOne.SetActive(true);
        yield return new WaitForSeconds(10f);
        EnemyTwo.SetActive(true);
        yield return new WaitForSeconds(10f);
        EnemyThree.SetActive(true);
    }


}

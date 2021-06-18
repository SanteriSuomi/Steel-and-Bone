using UnityEngine;

public class DeveloperTools : MonoBehaviour
{
    private SAMI_PlayerHealth health;

    private void Awake()
    {
        health = FindObjectOfType<SAMI_PlayerHealth>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            health.DeathEvent();
        }
    }
}
using UnityEngine;

public class EastTarget : MonoBehaviour
{
    public bool EastTriggered;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("PuzzleBoxGreen"))
        {
            EastTriggered = true;
        }
        else
        {
            EastTriggered = false;
        }
    }
}
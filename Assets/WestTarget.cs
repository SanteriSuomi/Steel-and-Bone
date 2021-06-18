using UnityEngine;

public class WestTarget : MonoBehaviour
{
    public bool WestTriggered;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("PuzzleBoxOrange"))
        {
            WestTriggered = true;
        }
        else
        {
            WestTriggered = false;
        }
    }
}
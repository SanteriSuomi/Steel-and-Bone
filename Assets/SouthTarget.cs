using UnityEngine;

public class SouthTarget : MonoBehaviour
{
    public bool SouthTriggered;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("PuzzleBox"))
        {
            SouthTriggered = true;
        }
        else
        {
            SouthTriggered = false;
        }
    }
}
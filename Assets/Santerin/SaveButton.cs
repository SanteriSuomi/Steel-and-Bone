using Essentials.Saving;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveButton : MonoBehaviour, IPointerClickHandler
{
    private bool canSave;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!SaveSystem.IsSaving && canSave)
        {
            canSave = false; // Prevent save button spamming, only be able to save once during every pause
            SaveManager.Instance.Save();
        }
    }

    private void OnEnable()
    {
        canSave = true;
    }
}
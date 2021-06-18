using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropdownImageEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TextMeshProUGUI itemLabel = default;
    [SerializeField]
    private GameObject saveImage = default;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (MainMenuManager.SaveScreenshotExists
            && itemLabel.text == "Load Existing Game")
        {
            saveImage.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        saveImage.SetActive(false);
    }
}
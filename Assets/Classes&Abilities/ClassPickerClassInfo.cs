using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventSystem;

public class ClassPickerClassInfo : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField]
    private ClassBase classToActivate = default;
    [SerializeField]
    private TextMeshProUGUI infoText = default;
    [SerializeField]
    private float infoTextYOffset = 25;
    [SerializeField]
    private AbilityBarHandler m_AbilityBarHandler = default;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //infoText.gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        classToActivate.Activate();
        m_AbilityBarHandler.UpdateAbilityBar();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //infoText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (current.IsPointerOverGameObject())
        {
            infoText.rectTransform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y + infoTextYOffset, 0);
        }

        if (Gamepad.current?.dpad.right.wasPressedThisFrame == true
            && (classToActivate is WarriorClass))
        {
            classToActivate.Activate();
        }
        else if (Gamepad.current?.dpad.left.wasPressedThisFrame == true
                 && (classToActivate is RogueClass))
        {
            classToActivate.Activate();
        }
    }
}
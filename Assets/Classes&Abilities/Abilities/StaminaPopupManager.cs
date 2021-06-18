using System.Collections;
using TMPro;
using UnityEngine;

public class StaminaPopupManager : Singleton<StaminaPopupManager>
{
    [SerializeField]
    private float popupWaitForSecondsTime = 1.5f;
    private TextMeshProUGUI noStaminaPopup;
    private WaitForSeconds popupWaitForSeconds;
    private Coroutine popupCoroutine;

    protected override void Awake()
    {
        noStaminaPopup = GetComponentInChildren<TextMeshProUGUI>(true);
        popupWaitForSeconds = new WaitForSeconds(popupWaitForSecondsTime);
    }

    public void Activate()
    {
        if (popupCoroutine != null)
        {
            StopCoroutine(popupCoroutine);
        }

        popupCoroutine = StartCoroutine(StartActivateCoroutine());
    }

    private IEnumerator StartActivateCoroutine()
    {
        noStaminaPopup.gameObject.SetActive(true);
        yield return popupWaitForSeconds;
        noStaminaPopup.gameObject.SetActive(false);
    }
}

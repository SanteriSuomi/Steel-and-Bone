using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameDropdownController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private float timeToDisable = 4;
    private float timer;

    private bool mouseIsOverDropdown;

    private void OnEnable()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        timer = 0;
        while (enabled)
        {
            timer += Time.deltaTime;
            if (timer >= timeToDisable / 2
                && !mouseIsOverDropdown
                && transform.childCount < 5)
            {
                gameObject.SetActive(false);
            }

            yield return null;
        }
    }

    private void OnDisable()
    {
        StopCoroutine(Countdown());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        timer = 0;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseIsOverDropdown = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseIsOverDropdown = false;
    }
}
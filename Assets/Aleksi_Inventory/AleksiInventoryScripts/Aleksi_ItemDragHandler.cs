using UnityEngine;
using UnityEngine.EventSystems;
//using static InventoryItem;

public class Aleksi_ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
	public IInventoryItem Item { get; set; }

	public void OnDrag(PointerEventData eventData)
	{
		transform.position = Input.mousePosition;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		transform.localPosition = Vector3.zero;
	}
}
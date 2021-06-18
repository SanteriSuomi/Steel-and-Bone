using UnityEngine;
using UnityEngine.UI;
//using static InventoryItem;

public class Aleksi_HUD : MonoBehaviour
{
	public Aleksi_Inventory Inventory;
	public GameObject MessagePanel;
	public AudioClip firstAudioClip;
	public AudioClip SecondAudioClip;
	public AudioSource audioSource;
	private Transform inventory;
	[SerializeField]
	private Image[] inventoryImageSlots = default;
	private Aleksi_ItemDragHandler[] inventoryDragHandlers;
	private Aleksi_ItemClickHandler[] inventoryClickHandlers;

	private void OnEnable()
	{
		Inventory.ItemAdded += InventoryScript_ItemAdded;
		//Inventory.ItemRemoved += Inventory_ItemRemoved;

		inventoryDragHandlers = new Aleksi_ItemDragHandler[inventoryImageSlots.Length];
		for (int i = 0; i < inventoryDragHandlers.Length; i++)
		{
			inventoryDragHandlers[i] = inventoryImageSlots[i].GetComponent<Aleksi_ItemDragHandler>();
		}

		inventoryClickHandlers = new Aleksi_ItemClickHandler[inventoryImageSlots.Length];
		for (int i = 0; i < inventoryClickHandlers.Length; i++)
		{
			inventoryClickHandlers[i] = inventoryImageSlots[i].transform.parent.GetComponent<Aleksi_ItemClickHandler>();
		}

		audioSource = GetComponent<AudioSource>();
		inventory = transform.Find("Inventory");
	}

	private void InventoryScript_ItemAdded(object sender, InventoryEventArgs invArgs)
	{
		if (invArgs.IsSaveEventArgs)
		{
			Image imageslot = inventoryImageSlots[invArgs.ItemSlotIndex];
			imageslot.enabled = true;
			imageslot.sprite = invArgs.Item.Image;
			inventoryDragHandlers[invArgs.ItemSlotIndex].Item = invArgs.Item;
			if (invArgs.Item.ItemType != ItemType.HealthPotion
				&& invArgs.Item.ItemType == InventoryItemBase.CurrentlyEquippedItem)
			{                
				inventoryClickHandlers[invArgs.ItemSlotIndex].OnItemClicked(invArgs.Item);
			}
		}
		else
		{
			foreach (Transform slot in inventory)
			{
				Transform imageTransform = slot.GetChild(0).GetChild(0);
				Image image = imageTransform.GetComponent<Image>();
				Aleksi_ItemDragHandler itemDragHandler = imageTransform.GetComponent<Aleksi_ItemDragHandler>();
				if (!image.enabled)
				{
					image.enabled = true;
					image.sprite = invArgs.Item.Image;
					itemDragHandler.Item = invArgs.Item;
					break;
				}
			}
		}

		if (invArgs.PlayItemAddSound)
		{
			audioSource.PlayOneShot(SecondAudioClip);
		}
	}

	//private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
	//{
	//	Transform inventoryPanel = transform.Find("Inventory");
	//	foreach (Transform slot in inventoryPanel)
	//	{
	//		print(slot);
	//		if (slot.GetChild(0).GetChild(0).GetComponent<Image>().sprite != null)
	//		{
	//			Transform imageTransform = slot.GetChild(0).GetChild(0);
	//			Image image = imageTransform.GetComponent<Image>();
	//			Aleksi_ItemDragHandler itemDragHandler = imageTransform.GetComponent<Aleksi_ItemDragHandler>();

	//			if (itemDragHandler.Item.Equals(e.Item))
	//			{
	//				image.enabled = false;
	//				image.sprite = null;
	//				itemDragHandler.Item = null;
	//				break;
	//			}
	//		}
	//	}
	//}

	public void OpenMessagePanel(string text)
	{
		MessagePanel.SetActive(true);
	}

	public void CloseMessagePanel()
	{
		MessagePanel.SetActive(false);
	}
}
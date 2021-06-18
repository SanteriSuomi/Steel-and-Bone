using Essentials.Saving;
using MessagePack;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Aleksi_Inventory : MonoBehaviour, ISaveable
{
	public static IInventoryItem[] Inventory { get; private set; } = new IInventoryItem[AMOUNT_OF_SLOTS];

	private const int AMOUNT_OF_SLOTS = 5;

	[SerializeField]
	private Image[] slotImages = default;

	public event EventHandler<InventoryEventArgs> ItemAdded;
	public event EventHandler<InventoryEventArgs> ItemRemoved;
	public event EventHandler<InventoryEventArgs> ItemUsed;

	public AudioClip SecondAudioClip;
	public AudioSource audioSource;

	private void Awake()
	{
		SaveSystem.Register(this);

		audioSource = GetComponent<AudioSource>();
	}

	#region Saving
	public SaveData GetSave()
	{
		ItemType[] itemTypes = new ItemType[Inventory.Length];
		for (int i = 0; i < Inventory.Length; i++)
		{
			if (Inventory[i] == null)
			{
				itemTypes[i] = ItemType.None;
			}
			else
			{
				itemTypes[i] = Inventory[i].ItemType;
			}
		}

		return new InventoryData(gameObject.name)
		{
			itemTypes = itemTypes,
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is InventoryData save)
		{
			Inventory = new IInventoryItem[AMOUNT_OF_SLOTS];
			for (int i = 0; i < Inventory.Length; i++)
			{
				if (save.itemTypes[i] != ItemType.None
					&& InventoryItemDatabase.AllItems.TryGetValue(save.itemTypes[i], out IInventoryItem inventoryItem))
				{
					GameObject invItemGameObj = (inventoryItem as MonoBehaviour).gameObject;
					if (invItemGameObj != null)
					{
						GameObject newInvItemGameObj = Instantiate(invItemGameObj); // Instantiate a new copy of the stored inventory item
						AddItem(newInvItemGameObj.GetComponent<IInventoryItem>(), (true, i), false, true);
					}
					else
					{
						Debug.LogWarning("IInventoryItem does not have a GameObject.");
					}
				}
			}
		}
	}

	[Serializable, MessagePackObject]
	public class InventoryData : SaveData
	{
		[Key("ItemTypes")]
		public ItemType[] itemTypes;

		public InventoryData() { }

		public InventoryData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion

	public void AddItem(IInventoryItem item, (bool useIndex, int index) indexData, bool playSound, bool isSaveEventArgs)
	{
		(int itemCount, int firstOpenIndex) = GetItemsData();
		if (itemCount < AMOUNT_OF_SLOTS)
		{
			// Catch exception because the gameObj gets destroyed while getting the colliders, which causes a null reference exception. This is (at least) a temporary fix.
			try
			{
				MonoBehaviour monoItem = item as MonoBehaviour;
				if (monoItem == null) return;

				List<Collider> colliders = new List<Collider>();
				if (monoItem.TryGetComponent(out Collider mainCollider))
				{
					colliders.Add(mainCollider);
				}

				int childCount = monoItem.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = monoItem.transform.GetChild(i);
					if (child == null)
					{
						return;
					}
					else if (child.TryGetComponent(out Collider childCollider))
					{
						colliders.Add(childCollider);
					}
				}

				bool canPickup = false;
				for (int i = 0; i < colliders.Count; i++)
				{
					if (colliders[i].enabled)
					{
						colliders[i].enabled = false;
						canPickup = true;
					}
				}

				if (canPickup)
				{
					if (indexData.useIndex)
					{
						Inventory[indexData.index] = item;
					}
					else
					{
						Inventory[firstOpenIndex] = item;
					}

					item.OnPickup();
					ItemAdded?.Invoke(this, new InventoryEventArgs(item)
					{
						IsSaveEventArgs = isSaveEventArgs,
						ItemSlotIndex = indexData.index,
						PlayItemAddSound = playSound
					});
				}
			}
			catch (NullReferenceException e)
			{
				Debug.LogWarning(e);
			}

		}
	}

	private (int, int) GetItemsData()
	{
		int itemCount = 0;
		int firstOpenIndex = -1;
		for (int i = 0; i < Inventory.Length; i++)
		{
			if (Inventory[i] == null
				&& firstOpenIndex == -1)
			{
				firstOpenIndex = i;
			}
			else
			{
				itemCount++;
			}
		}

		return (itemCount, firstOpenIndex);
	}

	internal void UseItem(IInventoryItem item, bool playSound)
	{
		if (playSound)
		{
			//audioSource.PlayOneShot(SecondAudioClip, 0.5f);
		}

		ItemUsed?.Invoke(this, new InventoryEventArgs(item));
	}

	public void RemoveItem(IInventoryItem item)
	{
		int itemIndex = GetItemIndex(item);
		if (itemIndex >= 0)
		{
			Inventory[itemIndex] = null;
			slotImages[itemIndex].enabled = false;

			audioSource.PlayOneShot(SecondAudioClip);

			GameObject itemGameObj = (item as MonoBehaviour).gameObject;
			if (item.ItemType == ItemType.HealthPotion
				&& itemGameObj != null)
			{
				Destroy(itemGameObj);
			}
			else
			{
				item.OnDrop();
			}

			if (itemGameObj != null
				&& itemGameObj.TryGetComponent(out Collider collider))
			{
				collider.enabled = true;
			}

			ItemRemoved?.Invoke(this, new InventoryEventArgs(item));
		}
	}

	private int GetItemIndex(IInventoryItem item)
	{
		int itemIndex = -1;
		for (int i = 0; i < Inventory.Length; i++)
		{
			if (Inventory[i] == item)
			{
				itemIndex = i;
				break;
			}
		}

		return itemIndex;
	}
}
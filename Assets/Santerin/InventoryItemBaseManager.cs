using Essentials.Saving;
using MessagePack;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemBaseManager : MonoBehaviour, ISaveable
{
	private void Awake()
	{
		SaveSystem.Register(this);
	}

	private void OnEnable()
	{
		InventoryItemBase.CurrentlyEquippedItem = ItemType.None;
		InventoryItemBase.CurrentItems = new List<ItemType>();
	}

	#region Saving
	public SaveData GetSave()
	{
		int[] currentPickupAsInt = new int[InventoryItemBase.CurrentItems.Count];
		for (int i = 0; i < currentPickupAsInt.Length; i++)
		{
			currentPickupAsInt[i] = (int)InventoryItemBase.CurrentItems[i];
		}

		return new InventoryItemBaseData(gameObject.name)
		{
			currentlyEquippedWeapon = (int)InventoryItemBase.CurrentlyEquippedItem,
			currentPickups = currentPickupAsInt
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is InventoryItemBaseData save)
		{
			InventoryItemBase.CurrentlyEquippedItem = (ItemType)save.currentlyEquippedWeapon;
			InventoryItemBase.CurrentItems = new List<ItemType>(save.currentPickups.Length);
			for (int i = 0; i < InventoryItemBase.CurrentItems.Count; i++)
			{
				InventoryItemBase.CurrentItems[i] = (ItemType)save.currentPickups[i];
			}
		}
	}

	[Serializable, MessagePackObject]
	public class InventoryItemBaseData : SaveData
	{
		[Key("EquippedWeapon")]
		public int currentlyEquippedWeapon;
		[Key("EquippedPickups")]
		public int[] currentPickups;

		public InventoryItemBaseData() { }

		public InventoryItemBaseData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion
}
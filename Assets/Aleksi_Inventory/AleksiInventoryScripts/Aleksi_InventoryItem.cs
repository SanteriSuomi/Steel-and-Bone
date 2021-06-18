using System;
using UnityEngine;

public interface IInventoryItem
{
	ItemType ItemType { get; }
    //PickupType ItemType { get; }
	string Name { get; }
	Sprite Image { get; }
	void OnPickup();
    void OnDrop();
	void OnUse();
}

public class InventoryEventArgs : EventArgs
{
	public InventoryEventArgs(IInventoryItem item)
	{
		Item = item;
	}

	public IInventoryItem Item { get; }

	// Used by saving
	public bool IsSaveEventArgs { get; set; }
	public int ItemSlotIndex { get; set; }
	public bool PlayItemAddSound { get; set; }
}
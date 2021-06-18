using System.Collections.Generic;
using UnityEngine;

// Note: This object is saved from InventoryItemBaseSaver.cs
public class InventoryItemBase : MonoBehaviour, IInventoryItem
{
	private PlayerAttack PlayerAttack;

	public Vector3 PickPosition;
	public Vector3 PickRotation;

	public virtual string Name
	{
		get => "_base item_";
	}

	public Sprite _Image;
	public Sprite Image
	{
		get => _Image;
	}

	[SerializeField]
	private ItemType itemType = default;
	public ItemType ItemType => itemType;
	public static ItemType CurrentlyEquippedItem { get; set; }
	public static List<ItemType> CurrentItems { get; set; } = new List<ItemType>();

	//[SerializeField]
 //   private PickupType pickupType = default;
 //   public PickupType ItemType => pickupType;

	private void Awake()
    {
        PlayerAttack = FindObjectOfType<PlayerAttack>();
    }

    public virtual void OnUse()
	{
		ModifyPlayerStatsBasedOnWeaponType();
	}

	public virtual void OnDrop()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, 100))
		{
			gameObject.SetActive(true);
			gameObject.transform.position = hit.point;
		}
	}

	public virtual void OnPickup()
	{
		gameObject.SetActive(false);
	}

	public void ModifyPlayerStatsBasedOnWeaponType()
	{
		if (ItemType == ItemType.HealthPotion) return;

		PlayerAttack.m_DamageDone = PlayerAttack.DefaultDamageDone;
		switch (itemType)
		{
			case ItemType.Dagger:
                PlayerAttack.m_DamageDone += 2;
                PlayerAttack.m_MeleeStaminaDrain = 15;
				break;

			case ItemType.Sword:
                PlayerAttack.m_DamageDone += 6;
                PlayerAttack.m_MeleeStaminaDrain = 25;
				break;

			case ItemType.SmallAxe:
                PlayerAttack.m_DamageDone += 8;
                PlayerAttack.m_MeleeStaminaDrain = 30;
				break;

			case ItemType.BigAxe:
                PlayerAttack.m_DamageDone += 10;
                PlayerAttack.m_MeleeStaminaDrain = 40;
				break;

			case ItemType.Bow:
                PlayerAttack.m_DamageDone += 5;
                PlayerAttack.m_MeleeStaminaDrain = 20;
				break;

			case ItemType.MagicAxe:
                PlayerAttack.m_DamageDone += 12;
                PlayerAttack.m_MeleeStaminaDrain = 45;
				break;
		}

		CurrentlyEquippedItem = itemType;
	}
}
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemDatabase : MonoBehaviour
{
    public static Dictionary<ItemType, IInventoryItem> AllItems { get; private set; }

    [SerializeField]
    private GameObject[] items = default;

    private void Awake()
    {
        AllItems = new Dictionary<ItemType, IInventoryItem>();
        for (int i = 0; i < items.Length; i++)
        {
            GameObject item = Instantiate(items[i]);
            item.transform.position = Vector3.zero;
            IInventoryItem invItem = item.GetComponent<IInventoryItem>();
            AllItems.Add(invItem.ItemType, invItem);
            item.SetActive(false);
        }
    }
}
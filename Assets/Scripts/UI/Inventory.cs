using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private List<ItemData> ownedItems = new List<ItemData>();
    public List<ItemData> OwnedItems => ownedItems;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(ItemData item)
    {
        ownedItems.Add(item);
        Debug.Log("Picked up: " + item.itemName);

        RefreshUIIfOpen();
    }

    public void RemoveItem(ItemData item)
    {
        if (ownedItems.Contains(item))
        {
            ownedItems.Remove(item);
            Debug.Log("Removed from inventory: " + item.itemName);

            RefreshUIIfOpen();
        }
    }

    private void RefreshUIIfOpen()
    {
        MenuWindowUI menu = FindObjectOfType<MenuWindowUI>();
        if (menu != null && MenuWindowUI.IsOpen)
        {
            menu.RefreshInventoryUI();
        }
    }
}

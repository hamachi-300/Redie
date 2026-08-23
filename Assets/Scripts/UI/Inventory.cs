using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    // Static list that survives scene changes to persist items
    private static List<ItemData> persistentItems = new List<ItemData>();

    [SerializeField] private List<ItemData> ownedItems = new List<ItemData>();
    public List<ItemData> OwnedItems => ownedItems;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Restore persistent items into the active scene's inventory
            ownedItems.Clear();
            ownedItems.AddRange(persistentItems);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(ItemData item)
    {
        ownedItems.Add(item);
        persistentItems.Add(item);
        Debug.Log("Picked up: " + item.itemName);

        RefreshUIIfOpen();
    }

    public void RemoveItem(ItemData item)
    {
        if (ownedItems.Contains(item))
        {
            ownedItems.Remove(item);
            persistentItems.Remove(item);
            Debug.Log("Removed from inventory: " + item.itemName);

            RefreshUIIfOpen();
        }
    }

    public void ClearInventory()
    {
        ownedItems.Clear();
        persistentItems.Clear();
        RefreshUIIfOpen();
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

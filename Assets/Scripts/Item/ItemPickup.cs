using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Persistence settings")]
    [SerializeField] private string uniqueID; // Unique identifier for this pickup instance (can leave empty for auto-generated fallback)

    [Header("Item settings")]
    [SerializeField] private ItemData itemData; // Works for WeaponData or PotionData!

    // Static set of all item unique IDs that have been picked up
    private static HashSet<string> pickedUpItems = new HashSet<string>();

    public static HashSet<string> PickedUpItems
    {
        get => pickedUpItems;
        set => pickedUpItems = value;
    }

    public static void ClearPickedUpItems()
    {
        pickedUpItems.Clear();
    }

    private bool isPlayerInRange = false;

    private void Start()
    {
        string id = GetUniqueID();
        if (pickedUpItems.Contains(id))
        {
            // If this item was already picked up, destroy it immediately on scene load
            Destroy(gameObject);
        }
    }

    private string GetUniqueID()
    {
        if (!string.IsNullOrEmpty(uniqueID))
        {
            return uniqueID;
        }
        // Auto-generated fallback unique ID based on scene, name and position
        return gameObject.scene.name + "_" + gameObject.name + "_" + transform.position.x + "_" + transform.position.y;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (Inventory.Instance != null && itemData != null)
            {
                Inventory.Instance.AddItem(itemData);
                
                // Add to static picked-up list before destroying
                string id = GetUniqueID();
                pickedUpItems.Add(id);

                Destroy(gameObject); // Destroy the world item
            }
        }
    }

    private void OnGUI()
    {
        if (isPlayerInRange && itemData != null)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 50), 
                      "Press 'E' to pick up " + itemData.itemName, style);
        }
    }
}

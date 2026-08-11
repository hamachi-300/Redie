using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData; // Works for WeaponData or PotionData!

    private bool isPlayerInRange = false;

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

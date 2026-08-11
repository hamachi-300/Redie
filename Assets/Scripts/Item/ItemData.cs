using UnityEngine;

public class ItemData : ScriptableObject
{
    [Header("General Item Info")]
    public string itemName;
    public Sprite itemSprite;
    [TextArea] public string itemDescription;

    // Triggered when clicking the item slot inside the Inventory tab
    public virtual void Use(PlayerController player)
    {
        Debug.Log("Using generic item: " + itemName);
    }
}

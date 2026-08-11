using UnityEngine;

[CreateAssetMenu(fileName = "NewPotionData", menuName = "ScriptableObjects/HP Potion")]
public class PotionData : ItemData // Inherits from ItemData!
{
    [Header("Heal Stats")]
    public float healAmount = 25f;

    public override void Use(PlayerController player)
    {
        player.Heal(healAmount);
        
        if (Inventory.Instance != null)
        {
            Inventory.Instance.RemoveItem(this);
        }
    }
}

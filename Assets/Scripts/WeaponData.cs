using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "ScriptableObjects/Weapon Data")]
public class WeaponData : ItemData // Inherits from ItemData now!
{
    [Header("Weapon Config")]
    public RuntimeAnimatorController animatorOverride;
    public Vector3 localScale = Vector3.one;

    [Header("Stamina Stats")]
    public float staminaCostLight = 10f;
    public float staminaCostHeavy = 20f;

    [Header("Damage Stats")]
    public float lightAttackDamage = 15f;
    public float heavyAttackDamage = 35f;

    public override void Use(PlayerController player)
    {
        player.EquipWeapon(this); // Equips the weapon
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "ScriptableObjects/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("General Info")]
    public string weaponName;
    public Sprite weaponSprite;
    public RuntimeAnimatorController animatorOverride;

    public Vector3 localScale = Vector3.one;

    [Header("Stamina Stats")]
    public float staminaCostLight = 10f;
    public float staminaCostHeavy = 20f;
}

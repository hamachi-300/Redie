using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColliderControl : MonoBehaviour
{
    [Header("Default Unarmed Settings")]
    [SerializeField] private float defaultLightDamage = 5f;
    [SerializeField] private float defaultHeavyDamage = 15f;

    private PlayerController player;
    private List<Collider2D> alreadyHitTargets = new List<Collider2D>();

    void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    void OnEnable()
    {
        alreadyHitTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Prevent hitting the same enemy multiple times in a single swing frame
            if (alreadyHitTargets.Contains(other)) return;
            alreadyHitTargets.Add(other);

            // 1. Get the EnemyHealth script on the target
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null && player != null)
            {
                // 2. Check if we are currently performing a Heavy Attack
                bool isHeavy = false;
                if (player.PlayerAnimator != null)
                {
                    isHeavy = player.PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("HeavyAttack");
                }

                // 3. Determine and apply damage
                float damage = 0f;
                if (player.EquippedWeapon != null)
                {
                    damage = isHeavy ? player.EquippedWeapon.heavyAttackDamage : player.EquippedWeapon.lightAttackDamage;
                }
                else
                {
                    // Default unarmed (fists) damage values when no weapon is equipped
                    damage = isHeavy ? defaultHeavyDamage : defaultLightDamage;
                }

                enemyHealth.TakeDamage(damage);
            }
        }
    }

    public void ResetHitbox()
    {
        alreadyHitTargets.Clear();
    }
}

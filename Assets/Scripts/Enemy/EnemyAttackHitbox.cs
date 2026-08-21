using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private Collider2D hitCollider;
    private float currentDamage = 10f;

    void Start()
    {
        hitCollider = GetComponent<Collider2D>();
        
        // Start with the collider disabled so it only deals damage during a swing!
        if (hitCollider != null)
        {
            hitCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If it touches the player, apply damage!
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(currentDamage);
            }
        }
    }

    // Set the damage value dynamically (called by EnemyAI)
    public void SetDamage(float damage)
    {
        currentDamage = damage;
    }

    // Turn collider on at the hit frame
    public void EnableHitbox()
    {
        if (hitCollider != null) hitCollider.enabled = true;
    }

    // Turn collider off after the swing
    public void DisableHitbox()
    {
        if (hitCollider != null) hitCollider.enabled = false;
    }
}

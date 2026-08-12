using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    [Header("Death Animation Settings")]
    [SerializeField] private string deathTriggerName = "Die";
    [SerializeField] private bool destroyOnDeath = false; // Toggle to leave corpse in the scene or destroy it
    [SerializeField] private float deathDelay = 1.0f; // Time in seconds to wait before destroying (if enabled)

    private Animator animator;
    private EnemyAI enemyAI;
    private Collider2D enemyCollider;
    private Rigidbody2D rb2d;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Cache components
        animator = GetComponentInChildren<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        enemyCollider = GetComponent<Collider2D>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // Ignore damage if already dead

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        
        Debug.Log(gameObject.name + " hit! HP: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " has died!");

        // 1. Play the death animation trigger
        if (animator != null)
        {
            animator.SetTrigger(deathTriggerName);
        }

        // 2. Disable the AI movement brain so it stops chasing/acting
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        // 3. Disable collision so the player can walk through the corpse
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        // 4. Stop any sliding velocity immediately
        if (rb2d != null)
        {
            rb2d.velocity = Vector2.zero;
            rb2d.bodyType = RigidbodyType2D.Kinematic; // Prevent external physics forces
        }

        // 5. Optionally destroy the enemy GameObject after the animation delay
        if (destroyOnDeath)
        {
            Destroy(gameObject, deathDelay);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Persistence Settings")]
    [SerializeField] private string uniqueID; // Unique identifier (can leave empty for auto-generated coordinates fallback)

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    [Header("Death Animation Settings")]
    [SerializeField] private string deathTriggerName = "Die";
    [SerializeField] private bool destroyOnDeath = false; // Toggle to leave corpse in the scene or destroy it
    [SerializeField] private float deathDelay = 1.0f; // Time in seconds to wait before destroying (if enabled)

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound; // เพิ่มไฟล์เสียงตอนมอนสเตอร์โจมตี
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    // Static set of all defeated enemy unique IDs
    private static HashSet<string> defeatedEnemies = new HashSet<string>();

    public static HashSet<string> DefeatedEnemies
    {
        get => defeatedEnemies;
        set => defeatedEnemies = value;
    }

    public static void ClearDefeatedEnemies()
    {
        defeatedEnemies.Clear();
    }

    private Animator animator;
    private EnemyAI enemyAI;
    private Collider2D enemyCollider;
    private Rigidbody2D rb2d;
    private bool isDead = false;

    private void Start()
    {
        // 0. Check if this enemy has already been defeated previously
        string id = GetUniqueID();
        if (defeatedEnemies.Contains(id))
        {
            Destroy(gameObject);
            return;
        }

        currentHealth = maxHealth;

        // Cache components
        animator = GetComponentInChildren<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        enemyCollider = GetComponent<Collider2D>();
        rb2d = GetComponent<Rigidbody2D>();

        // Cache AudioSource if not manually assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private string GetUniqueID()
    {
        if (!string.IsNullOrWhiteSpace(uniqueID))
        {
            return uniqueID.Trim();
        }
        
        // Build hierarchy path to distinguish between enemies with the same name under different parents
        string path = gameObject.name;
        Transform t = transform.parent;
        while (t != null)
        {
            path = t.name + "/" + path;
            t = t.parent;
        }

        // Round coordinates to 2 decimal places to avoid tiny float variations causing mismatch
        float roundedX = Mathf.Round(transform.position.x * 100f) / 100f;
        float roundedY = Mathf.Round(transform.position.y * 100f) / 100f;

        return gameObject.scene.name + "_" + path + "_" + roundedX + "_" + roundedY;
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
        else
        {
            // เล่นเสียงโดนตี (กรณีมอนสเตอร์ยังไม่ตาย)
            PlayHurtSound();
        }
    }

    public void PlayAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void PlayHurtSound()
    {
        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " has died!");

        // Record that this enemy is defeated
        string id = GetUniqueID();
        defeatedEnemies.Add(id);

        // 0. Play death SFX
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

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

    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
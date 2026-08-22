using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float sprintStaminaDrainRate = 15f;

    private float currentStamina;
    private bool isDie = false;

    private PlayerController playerController;

    // Getters
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float MaxStamina => maxStamina;
    public float CurrentStamina => currentStamina;
    public float SprintStaminaDrainRate => sprintStaminaDrainRate;

    void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        playerController = GetComponent<PlayerController>();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log("Healed! Current HP: " + currentHealth + "/" + maxHealth);
    }

    public void TakeDamage(float damage, bool isInvincible)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (currentHealth <= 0f && !isDie)
        {
            isDie = true;
            Die();
        }
    }

    // Deducts stamina if there is enough, returns true if successful
    public bool ConsumeStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            return true;
        }
        return false;
    }

    // Direct subtraction for sprinting
    public void DrainSprintStamina()
    {
        currentStamina -= sprintStaminaDrainRate * Time.deltaTime;
        currentStamina = Mathf.Max(currentStamina, 0f);
    }

    // Handles automatic stamina regeneration
    public void RegenerateStamina(bool isSprinting)
    {
        if (currentStamina < maxStamina && !isSprinting)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    public void SetStats(float health, float stamina)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
        currentStamina = Mathf.Clamp(stamina, 0f, maxStamina);
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        playerController.Die();
    }
}

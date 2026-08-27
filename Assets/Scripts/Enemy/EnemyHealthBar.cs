using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI Slider")]
    [SerializeField] private Slider healthSlider;

    [Header("Enemy Health Reference")]
    [SerializeField] private EnemyHealth enemyHealth;

    private void Start()
    {
        // Automatically find components if not manually assigned
        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>();
        }

        if (enemyHealth == null)
        {
            enemyHealth = GetComponentInParent<EnemyHealth>();
        }

        // Initialize Slider value
        if (enemyHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = enemyHealth.GetMaxHealth();
            healthSlider.value = enemyHealth.GetHealth();
        }
        healthSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (enemyHealth != null && healthSlider != null)
        {
            float currentHP = enemyHealth.GetHealth();
            healthSlider.value = currentHP;

            // Optional: Hide the health bar when the enemy dies
            if (currentHP <= 0f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void SetVisible() {
        healthSlider.gameObject.SetActive(true);
    }

    public void SetInvisible() {
        healthSlider.gameObject.SetActive(false);
    }
}

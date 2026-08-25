using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossUIController : MonoBehaviour
{
    public static BossUIController Instance { get; private set; }

    [Header("UI Panels")]
    [Tooltip("Drag the main UI panel containing the Boss Health Bar here.")]
    [SerializeField] private GameObject bossUIPanel;

    [Tooltip("Drag the UI Slider component for the Health Bar here.")]
    [SerializeField] private Slider healthSlider;

    [Tooltip("Drag the TMPro Text component for the Boss Name here.")]
    [SerializeField] private TextMeshProUGUI bossNameText;

    [Header("Boss Reference")]
    [Tooltip("Drag the Boss GameObject (containing BossController) here.")]
    [SerializeField] private BossController boss;

    private void Awake()
    {
        // Set up Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Hide the UI by default at the start of the level
        if (bossUIPanel != null)
        {
            bossUIPanel.SetActive(false);
        }
    }

    public void ShowBossUI()
    {
        if (bossUIPanel != null)
        {
            bossUIPanel.SetActive(true);
        }
    }

    private void Update()
    {
        if (boss == null || bossUIPanel == null) return;

        // Only update values if the UI is active
        if (bossUIPanel.activeSelf)
        {
            // 1. Update health bar slider
            if (healthSlider != null && boss.GetMaxHp() > 0)
            {
                healthSlider.value = boss.GetHp() / boss.GetMaxHp();
            }

            // 2. Automatically hide the UI if the boss is dead
            if (boss.IsDead())
            {
                bossUIPanel.SetActive(false);
            }
        }
    }
}

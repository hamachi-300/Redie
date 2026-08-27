using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel; // The main "YOU DIED" panel
    [SerializeField] private Button respawnButton;    // Button to reload the last save

    [Header("Fallback Respawn Settings")]
    [SerializeField] private string fallbackSceneName = "StartCave";
    [SerializeField] private string fallbackSpawnPointName = "PlayerStartPoint";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Hide panel at start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Setup button listener and hide button at start
        if (respawnButton != null)
        {
            respawnButton.onClick.AddListener(OnRespawnClicked);
            respawnButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Activates the Game Over UI screen panel when called (e.g. from PlayerController when dying).
    /// </summary>
    public void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("GameOverUI: Screen displayed.");
        }
        if (respawnButton != null)
        {
            respawnButton.gameObject.SetActive(true);
        }
    }

    private void OnRespawnClicked()
    {
        // Hide the panel and button immediately
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (respawnButton != null)
        {
            respawnButton.gameObject.SetActive(false);
        }

        // Reset items, enemies, and inventory so the player restarts fresh
        ItemPickup.ClearPickedUpItems();
        EnemyHealth.ClearDefeatedEnemies();
        Inventory.ClearPersistentItems(); // Clear inventory on death respawn

        SceneTransition.nextSpawnPointName = fallbackSpawnPointName;
        SceneManager.LoadScene(fallbackSceneName);
    }
}

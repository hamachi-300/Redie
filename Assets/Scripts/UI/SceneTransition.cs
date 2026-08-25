using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes

public class SceneTransition : MonoBehaviour
{
    // Static variable that persists between scene loads
    public static string nextSpawnPointName;

    [Header("Scene Settings")]
    [SerializeField] private string targetSceneName; // Name of the scene to load
    [SerializeField] private string targetSpawnPointName; // Name of GameObject to spawn player at
    [SerializeField] private string Press_E_to = "Next Area"; // Name shown on screen (e.g. "SnowField", "Boss Cave")

    private bool isPlayerInside = false;

    void Update()
    {
        // If player is inside the trigger zone and presses E, teleport!
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            // Check if there is an active boss in the scene
            BossController boss = FindObjectOfType<BossController>();
            if (boss != null && boss.IsAwake() && !boss.IsDead())
            {
                Debug.LogWarning("Teleport blocked: Boss is active!");
                return; // Block the teleportation
            }

            Teleport();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    private void Teleport()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            // Store the next spawn point name before loading the scene
            nextSpawnPointName = targetSpawnPointName;
            
            Debug.Log("Teleporting player to scene: " + targetSceneName + " at spawn point: " + targetSpawnPointName);
            LoadingManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("No target scene name specified on " + gameObject.name);
        }
    }

    // Draws the E interaction prompt on the screen using the exact same GUI style as ItemPickup.cs
    private void OnGUI()
    {
        if (isPlayerInside)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            // Check if the boss is currently active
            BossController boss = FindObjectOfType<BossController>();
            bool isBossActive = boss != null && boss.IsAwake() && !boss.IsDead();

            string promptText = "Press 'E' to " + Press_E_to;
            if (isBossActive)
            {
                promptText = "Escape portal is BLOCKED during Boss Fight!";
                style.normal.textColor = Color.red; // Warn the player in red
            }

            GUI.Label(new Rect(Screen.width / 2 - 250, Screen.height - 100, 500, 50), 
                      promptText, style);
        }
    }
}

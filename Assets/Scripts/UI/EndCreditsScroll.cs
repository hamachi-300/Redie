using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Standard for text

public class EndCreditsScroll : MonoBehaviour
{
    [Header("Scroll Settings")]
    [SerializeField] private RectTransform creditsContainer; // Parent panel containing all text
    [SerializeField] private float scrollSpeed = 30f; // Pixels per second
    [SerializeField] private float autoSkipTime = 40f; // Auto returns to menu after this time

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI statsText; // Text showing player's victory stats

    [Header("Scene Config")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isScrollFinished = false;
    private float timer = 0f;

    void Start()
    {

        // Set victory stats automatically if a persistent stats script exists
        if (statsText != null)
        {
            // You can pull persistently saved player data here (e.g. PlayerPrefs or a static GameManager)
            int totalCoins = PlayerPrefs.GetInt("TotalCoinsCollected", 0);
            int levelsCleared = PlayerPrefs.GetInt("LevelsCleared", 1);
            
            statsText.text = "Re:DIE\n\n\n\n\n" +
                             "--- DEVELOPER ---\n" +
                             "Pluem\n\n\n\n\n" +
                             "--- LEVEL DESIGN ---\n" +
                             "Pluem\n\n\n\n\n" +
                             "--- ART & DESIGN ---\n" +
                             "Pluem & his AI friend\n\n\n\n\n" +
                             "--- SOUND & MUSIC ---\n" +
                             "ศุภกร เจริญวัย\n\n\n\n\n" +
                             "--- THANK FOR PLAYING ---\n\n\n\n\n";

            // Force Unity's layout system to calculate the text height immediately on frame 1
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(statsText.rectTransform);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 1. Scroll the container up
        if (creditsContainer != null && !isScrollFinished)
        {
            creditsContainer.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            // 2. Stop scrolling if container goes past the screen top height boundary plus padding
            // We read the actual height of the statsText rect in pixels
            float textHeight = (statsText != null) ? statsText.rectTransform.rect.height : creditsContainer.rect.height;
            if (creditsContainer.anchoredPosition.y >= textHeight)
            {
                ReturnToMainMenu();
            }
        }

        // 3. Auto skip fallback or manual ESC key check
        if (timer >= autoSkipTime || Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainMenu();
        }
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Loading main menu...");
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

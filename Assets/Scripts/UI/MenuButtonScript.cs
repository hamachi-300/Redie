using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtonScript : MonoBehaviour
{
    [Header("Button Setting")]
    [SerializeField] private Button newGameBtn;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button quitGameBtn;

    [Header("Scene Config")]
    [SerializeField] private string newGameSceneName;

    void Start()
    {
        // 1. New Game button listener
        if (newGameBtn != null)
        {
            newGameBtn.onClick.AddListener(NewGame);
        }

        // 2. Quit button listener
        if (quitGameBtn != null)
        {
            quitGameBtn.onClick.AddListener(QuitGame);
        }

        // 3. Continue button listener: enable/disable depending on save file existence
        bool hasSave = false;
        string savePath = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(savePath))
        {
            hasSave = true;
        }

        if (continueBtn != null)
        {
            if (hasSave)
            {
                continueBtn.interactable = true;
                continueBtn.onClick.AddListener(ContinueGame);
            }
            else
            {
                continueBtn.interactable = false; // Grayed out/unclickable
            }
        }
    }

    public void NewGame()
    {
        Debug.Log("Starting new game...");
        
        // Clear all saved status from previous run
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ClearSaveData();
        }
        else
        {
            // Fallback clear if manager hasn't instantiated yet
            string savePath = Path.Combine(Application.persistentDataPath, "save.json");
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
        }

        SceneManager.LoadScene(newGameSceneName);
    }

    public void ContinueGame()
    {
        Debug.Log("Continuing saved game...");
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.InitiateLoadGame();
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
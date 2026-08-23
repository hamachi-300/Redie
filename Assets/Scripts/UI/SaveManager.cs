using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Item Database")]
    [Tooltip("Drag all possible ItemData ScriptableObjects (weapons, potions, etc.) here so they can be loaded by name!")]
    [SerializeField] private List<ItemData> allPossibleItems = new List<ItemData>();

    [Header("Auto-Save Settings")]
    [SerializeField] private float autoSaveInterval = 1f; // Saves every 1 second
    [Tooltip("Only scenes listed here will trigger auto-saves (e.g. gameplay maps)")]
    [SerializeField] private List<string> playableScenes = new List<string> { "StartCave", "SnowField", "BossCave" };

    private string saveFilePath;
    private bool shouldLoadFromSave = false;
    private SaveData loadedSaveData = null;

    [System.Serializable]
    public class SaveData
    {
        public string currentSceneName;
        public float playerPosX;
        public float playerPosY;
        public float currentHealth;
        public float currentStamina;
        public List<string> inventoryItemNames = new List<string>();
        public List<string> pickedUpItemIDs = new List<string>();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
            Debug.Log("Save file path: " + saveFilePath);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Automatically search and load all ItemData assets inside any folder named "Resources"
        ItemData[] loadedItems = Resources.LoadAll<ItemData>("");
        allPossibleItems = new List<ItemData>(loadedItems);
        Debug.Log($"SaveManager: Automatically loaded {allPossibleItems.Count} items into the database.");

        StartCoroutine(AutoSaveLoop());
    }

    private IEnumerator AutoSaveLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            
            // Only auto-save if we are in a playable scene listed in the playableScenes database
            string activeScene = SceneManager.GetActiveScene().name;
            if (playableScenes.Contains(activeScene))
            {
                SaveGame();
            }
        }
    }

    public bool SaveFileExists()
    {
        return File.Exists(saveFilePath);
    }

    public void SaveGame()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null) return;

        SaveData data = new SaveData();
        data.currentSceneName = SceneManager.GetActiveScene().name;
        data.playerPosX = player.transform.position.x;
        data.playerPosY = player.transform.position.y;
        data.currentHealth = stats.CurrentHealth;
        data.currentStamina = stats.CurrentStamina;

        // Save inventory item names
        if (Inventory.Instance != null)
        {
            foreach (ItemData item in Inventory.Instance.OwnedItems)
            {
                if (item != null)
                {
                    data.inventoryItemNames.Add(item.itemName);
                }
            }
        }

        // Save permanently picked up items
        data.pickedUpItemIDs = new List<string>(ItemPickup.PickedUpItems);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Auto-Saved to " + saveFilePath);
    }



    public void ClearSaveData()
    {
        if (SaveFileExists())
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted.");
        }
        ItemPickup.ClearPickedUpItems();
    }

    public void InitiateLoadGame()
    {
        if (!SaveFileExists())
        {
            Debug.LogWarning("Cannot load game: Save file does not exist.");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        loadedSaveData = JsonUtility.FromJson<SaveData>(json);

        if (loadedSaveData != null && !string.IsNullOrEmpty(loadedSaveData.currentSceneName))
        {
            shouldLoadFromSave = true;
            Debug.Log("Initiating scene load: " + loadedSaveData.currentSceneName);
            SceneManager.LoadScene(loadedSaveData.currentSceneName);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (shouldLoadFromSave && loadedSaveData != null)
        {
            ApplyLoadedData();
            shouldLoadFromSave = false;
        }
    }

    private void ApplyLoadedData()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("Load Failed: PlayerController not found in loaded scene!");
            return;
        }

        // 1. Reposition Player
        Vector3 loadedPos = new Vector3(loadedSaveData.playerPosX, loadedSaveData.playerPosY, 0f);
        player.transform.position = loadedPos;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = loadedPos;
        }

        // 2. Restore Stats
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.SetStats(loadedSaveData.currentHealth, loadedSaveData.currentStamina);
        }

        // Restore permanently picked up items history
        if (loadedSaveData.pickedUpItemIDs != null)
        {
            ItemPickup.PickedUpItems = new HashSet<string>(loadedSaveData.pickedUpItemIDs);
        }
        else
        {
            ItemPickup.ClearPickedUpItems();
        }

        // 3. Restore Inventory Items
        if (Inventory.Instance != null)
        {
            Inventory.Instance.ClearInventory();
            foreach (string itemName in loadedSaveData.inventoryItemNames)
            {
                ItemData matchedItem = allPossibleItems.Find(item => item != null && item.itemName == itemName);
                if (matchedItem != null)
                {
                    Inventory.Instance.AddItem(matchedItem);
                }
                else
                {
                    Debug.LogWarning("SaveManager: Could not find ScriptableObject for item: " + itemName + " in the database array!");
                }
            }
        }

        Debug.Log("Save data successfully applied to Player and Inventory!");
    }
}

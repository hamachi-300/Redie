using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuWindowUI : MonoBehaviour
{
    [Header("Main Menu Panel")]
    [SerializeField] private GameObject menuPanel;

    [Header("Sub-Tab Panels (In Order: 0=Status, 1=Inventory, 2=Settings)")]
    [SerializeField] private GameObject[] tabPanels;

    [Header("Tab Header Buttons (Optional: for visual highlight)")]
    [SerializeField] private Button[] tabButtons;

    [Header("Tab Colors")]
    [SerializeField] private Color activeTabColor = new Color(0.35f, 0.35f, 0.4f, 1f);
    [SerializeField] private Color inactiveTabColor = new Color(0.25f, 0.25f, 0.3f, 1f);

    [Header("Inventory UI Elements")]
    [SerializeField] private GameObject slotPrefab; 
    [SerializeField] private Transform slotsParent;

    [Header("Settings Tab Elements")]
    [SerializeField] private Button exitToMenuButton;
    [SerializeField] private string mainMenuSceneName = "GameMenu";

    public static bool IsOpen { get; private set; } = false;
    private int currentTabIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
            IsOpen = false;
        }

        if (exitToMenuButton != null)
        {
            exitToMenuButton.onClick.AddListener(ExitToMainMenu);
        }

        // add listener for tab buttons
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i; // Cache index for lambda
            if (tabButtons[i] != null)
            {
                tabButtons[i].onClick.AddListener(() => SelectTab(index));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // press i to open 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }

        if (IsOpen)
        {
            // Right Arrow or 'E' key -> Next Tab (Right)
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.E))
            {
                SelectNextTab();
            }
            // Left Arrow or 'Q' key -> Previous Tab (Left)
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q))
            {
                SelectPreviousTab();
            }
        }
    }

    public void ToggleMenu()
    {
        if (menuPanel == null) return;

        bool newState = !menuPanel.activeSelf;
        menuPanel.SetActive(newState);
        IsOpen = newState;

        if (newState)
        {
            // Show current tab when opened
            SelectTab(currentTabIndex);
            RefreshInventoryUI();
        }
    }

    public void SelectTab(int tabIndex)
    {
        if (tabPanels == null || tabPanels.Length == 0) return;
        currentTabIndex = Mathf.Clamp(tabIndex, 0, tabPanels.Length - 1);
        // Hide all tabs, show selected tab
        for (int i = 0; i < tabPanels.Length; i++)
        {
            if (tabPanels[i] != null)
            {
                tabPanels[i].SetActive(i == currentTabIndex);
            }

            if (tabButtons != null && i < tabButtons.Length && tabButtons[i] != null)
            {
                Image btnImage = tabButtons[i].GetComponent<Image>();
                if (btnImage != null)
                {
                    btnImage.color = (i == currentTabIndex) ? activeTabColor : inactiveTabColor;
                }
            }
        }
    }

    public void SelectNextTab()
    {
        int nextIndex = (currentTabIndex + 1) % tabPanels.Length;
        SelectTab(nextIndex);
    }

    public void SelectPreviousTab()
    {
        int prevIndex = (currentTabIndex - 1 + tabPanels.Length) % tabPanels.Length;
        SelectTab(prevIndex);
    }

    public void RefreshInventoryUI()
    {
        if (slotsParent == null || slotPrefab == null) return;

        // Clear existing slot buttons
        foreach (Transform child in slotsParent)
        {
            Destroy(child.gameObject);
        }

        if (Inventory.Instance == null) return;
        List<ItemData> items = Inventory.Instance.OwnedItems;


        // Spawn a button for each item currently owned
        foreach (ItemData item in items)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotsParent);

            // Find child Icon image and set its sprite
            Transform iconChild = newSlot.transform.Find("InventoryItemIcon");
            Image slotIcon = (iconChild != null) ? iconChild.GetComponent<Image>() : newSlot.GetComponent<Image>();
            
            if (slotIcon != null)
            {
                slotIcon.sprite = item.itemSprite;
            }

            // Bind click to use the item
            Button btn = newSlot.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    PlayerController player = FindObjectOfType<PlayerController>();
                    if (player != null)
                    {
                        item.Use(player);
                    }
                });
            }
        }
    }

    private void ExitToMainMenu()
    {
        // 1. Force a save immediately before exiting
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }

        // 2. Hide menu panel
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
        IsOpen = false;

        // 3. Load game menu scene
        Debug.Log("Exiting to menu: " + mainMenuSceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }
}

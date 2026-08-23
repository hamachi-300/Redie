using UnityEngine;

public class MinimapToggle : MonoBehaviour
{
    [Header("Minimap UI Panel")]
    [Tooltip("Drag the Minimap_Border (or the main minimap parent object) here!")]
    [SerializeField] private GameObject minimapPanel;

    [Header("Toggle Setting")]
    [SerializeField] private KeyCode toggleKey = KeyCode.M;

    private void Start() {
        if (minimapPanel != null)
        {
            minimapPanel.SetActive(false); // Starts hidden by default
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (minimapPanel != null)
            {
                minimapPanel.SetActive(!minimapPanel.activeSelf);
            }
        }
    }
}

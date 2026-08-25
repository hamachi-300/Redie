using UnityEngine;

public class TeleportBlocker : MonoBehaviour
{
    [Header("Barricade / Gate Settings")]
    [Tooltip("The solid wall / barrier GameObject that physically blocks the arena exit.")]
    [SerializeField] private GameObject barrierObject;

    [Header("Boss Settings")]
    [Tooltip("Reference to the Boss. If left empty, the script will automatically find it in the scene.")]
    [SerializeField] private BossController boss;

    private void Start()
    {
        // Auto-find the boss if not dragged in the Inspector
        if (boss == null)
        {
            boss = FindObjectOfType<BossController>();
        }

        // Deactivate the physical wall at start (so the player can enter the arena)
        if (barrierObject != null)
        {
            barrierObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (boss == null) return;

        // If the boss is awake and alive, activate the wall to trap the player
        if (boss.IsAwake() && !boss.IsDead())
        {
            if (barrierObject != null && !barrierObject.activeSelf)
            {
                barrierObject.SetActive(true);
                Debug.Log("Boss fight active: Exit blocked!");
            }
        }
        // Otherwise (boss is sleeping or dead), disable the wall so they can leave
        else
        {
            if (barrierObject != null && barrierObject.activeSelf)
            {
                barrierObject.SetActive(false);
                Debug.Log("Boss fight inactive: exit opened.");
            }
        }
    }
}

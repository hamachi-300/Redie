using UnityEngine;

public class Totem : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("How close the player must be vertically (Y-axis) to activate the totem.")]
    [SerializeField] private float detectionRange = 2f;

    [Header("Animation Parameters")]
    [Tooltip("The Boolean parameter name set to true when activated, and false when the boss dies.")]
    [SerializeField] private string animationBoolName = "Lit";

    [Header("Boss Reference")]
    [Tooltip("Reference to the Boss. If left empty, will automatically find in the scene.")]
    [SerializeField] private BossController boss;

    private Animator animator;
    private GameObject player;
    private bool hasActivated = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (boss == null)
        {
            boss = FindObjectOfType<BossController>();
        }
    }

    private void Update()
    {
        // 1. If boss dies, turn off the Lit boolean and stop checks
        if (boss != null && boss.IsDead())
        {
            if (animator != null && !string.IsNullOrEmpty(animationBoolName))
            {
                animator.SetBool(animationBoolName, false);
            }
            this.enabled = false; // Disable script completely since boss is dead
            return;
        }

        // 2. If already activated and boss is still alive, do nothing
        if (hasActivated) return;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }

        // 3. Calculate distance strictly on Y-axis
        float distanceY = Mathf.Abs(transform.position.y - player.transform.position.y);
        bool isPlayerInRange = distanceY <= detectionRange;

        // 4. Activate totem (set Lit to true) when player enters vertical range
        if (isPlayerInRange)
        {
            if (animator != null && !string.IsNullOrEmpty(animationBoolName))
            {
                animator.SetBool(animationBoolName, true);
            }

            hasActivated = true;
        }
    }
}

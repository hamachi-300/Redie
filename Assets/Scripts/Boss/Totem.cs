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
    private AudioSource audioSource;
    private bool hasActivated = false; // Tracks if the fire has been lit

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();

        if (boss == null)
        {
            boss = FindObjectOfType<BossController>();
        }
    }

    private void Update()
    {
        // 1. If boss dies, extinguish the fire (Lit = false), stop sound, and shut down
        if (boss != null && boss.IsDead())
        {
            if (animator != null && !string.IsNullOrEmpty(animationBoolName))
            {
                animator.SetBool(animationBoolName, false);
            }
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            this.enabled = false; // Disable script completely since boss is dead
            return;
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }

        // 2. Calculate distance strictly on the Y-axis
        float distanceY = Mathf.Abs(transform.position.y - player.transform.position.y);
        bool isPlayerInRange = distanceY <= detectionRange;

        // 3. Ignite fire (one-time activation)
        if (isPlayerInRange && !hasActivated)
        {
            if (animator != null && !string.IsNullOrEmpty(animationBoolName))
            {
                animator.SetBool(animationBoolName, true); // Keep fire lit permanently
            }
            hasActivated = true;
        }

        // 4. Dynamic Audio Fading (Only works if the fire has already been lit!)
        if (hasActivated)
        {
            if (isPlayerInRange)
            {
                if (audioSource != null)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }

                    // Smooth Y-distance volume fading
                    float volumeRatio = 1f - (distanceY / detectionRange);
                    audioSource.volume = Mathf.Clamp01(volumeRatio);
                }
            }
            else
            {
                // Stop sound when player is out of range, but keep fire burning!
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
    }
}

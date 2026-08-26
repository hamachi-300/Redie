using UnityEngine;
using System.Collections;

public class JumpScare : MonoBehaviour
{
    public static JumpScare Instance { get; private set; }

    [Header("UI Reference")]
    [Tooltip("The full-screen GameObject panel containing the jumpscare image/animation.")]
    [SerializeField] private GameObject jumpScarePanel;

    [Header("Audio Reference")]
    [Tooltip("The AudioSource used to play the jumpscare sound.")]
    [SerializeField] private AudioSource audioSource;
    
    [Tooltip("The scary scream / shriek audio clip.")]
    [SerializeField] private AudioClip screamClip;

    [Header("Timing Settings")]
    [Tooltip("How long the jumpscare panel stays active on screen (in seconds).")]
    [SerializeField] private float scareDuration = 2.0f;

    private void Awake()
    {
        // Singleton pattern setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Hide the jumpscare panel at start
        if (jumpScarePanel != null)
        {
            jumpScarePanel.SetActive(false);
        }
    }

    // Public method called by the Boss when they die
    public void TriggerJumpScare()
    {
        StartCoroutine(JumpScareRoutine());
    }

    private IEnumerator JumpScareRoutine()
    {
        // 1. Wait for 1 second after the boss dies
        yield return new WaitForSeconds(1.0f);

        // 2. Play the jumpscare shriek/scream sound
        if (audioSource != null && screamClip != null)
        {
            // Set full volume for maximum effect!
            audioSource.PlayOneShot(screamClip, 1.0f);
        }

        // 3. Display the jumpscare visual panel
        if (jumpScarePanel != null)
        {
            jumpScarePanel.SetActive(true);
        }

        // 4. Wait for the jumpscare duration
        yield return new WaitForSeconds(scareDuration);

        // 5. Hide the jumpscare visual panel
        if (jumpScarePanel != null)
        {
            jumpScarePanel.SetActive(false);
        }
    }
}

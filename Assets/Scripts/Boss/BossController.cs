using System.Collections; // Required for IEnumerator
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Default Facing Direction")]
    [SerializeField] private Vector2 defaultFacingDirection = Vector2.down;

    private float sweepingAttackColdown = 2f;
    private float sweepingAttackTriggerRange = 3f;
    private float sweepingAttackColdownRemain = 0f;
    private float sweepingAttackTimer = 1f;
    private float attackTimer = 0f;
    private float sweepingAttackDamage = 10f;

    private float stabAttackColdown = 5f;
    private float stabAttackTriggerRange = 5f;
    private float stabAttackColdownRemain = 0f;
    private bool isChargeStab = false;
    private float stabAttackChargeTime = 1f;
    private float stabAttackChargeTimer = 1f;
    private float stabAttackSpeed = 10f;
    private bool isDashing = false;
    private Vector3 dashTargetPosition;
    private float dashTimer = 0f;
    private float maxDashDuration = 0.5f; // limit dash time for prevent bug with collider
    private float stabbingDamage = 20f;
    private bool hasHitThisDash = false;

    private float speed = 2.5f;
    private float stoppingDistance = 2f; 

    private float jumpAttackColdown = 8f;
    private float jumpAttackTriggerRange = 15f;
    private float jumpAttackColdownRemain = 0f;
    private bool isJumping = false;
    private int jumpPhase = 0; // 0 = none, 1 = jump up, 2 = jump down
    private float jumpTimer = 0f;
    [SerializeField] private float jumpHeight = 10f;         // Max height (increase this to jump higher!)
    [SerializeField] private float jumpRiseSpeed = 12f;       // Vertical rising speed
    [SerializeField] private float jumpFallSpeed = 20f;       // Vertical falling speed
    [SerializeField] private float jumpHorizontalSpeed = 3f;  // Speed multiplier for moving towards player's X/Y
    private float currentJumpHeight = 0f;
    private Vector3 jumpTargetPosition;
    private Vector3 jumpStartPosition;
    private float jumpAttackDamage = 40f;

    [Header("Jump Attack Effects")]
    [SerializeField] private string jumpImpactTriggerName = "Slam";

    [Header("Attack Audio Clips")]
    [SerializeField] private AudioClip sweepSound;
    [SerializeField] private AudioClip stabSound;
    [SerializeField] private AudioClip jumpUpSound;
    [SerializeField] private AudioClip jumpDownSound;

    private float sleepTime = 3f;
    private float visibleRange = 10f;

    private float maxHp = 300f;
    private float hp = 0;

    private bool isDie = false;
    private bool isSleeping = true;

    [Header("Attack Hitboxes")]
    [SerializeField] private EnemyAttackHitbox sweepHitbox;
    [SerializeField] private EnemyAttackHitbox stabHitbox;
    [SerializeField] private EnemyAttackHitbox jumpHitbox;
    
    private Animator animator;
    private GameObject player;
    private Rigidbody2D rb2d;
    private Collider2D myCollider;
    private AudioSource audioSource;
    

    private IEnumerator Start()
    {
        hp = 0f;
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        rb2d = GetComponent<Rigidbody2D>();
        myCollider = GetComponentInChildren<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        // boss see player then awake from coffin
        while (player == null || Vector3.Distance(transform.position, player.transform.position) > visibleRange)
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            
            yield return null; // Wait for the next frame before checking again
        }

        // awake from coffin when player in visible range
        yield return new WaitForSeconds(sleepTime);
        if (animator != null)
        {
            animator.SetTrigger("Awake");
            animator.SetFloat("MoveX", defaultFacingDirection.x);
            animator.SetFloat("MoveY", defaultFacingDirection.y);
        }

        // Show the Boss HP bar UI!
        if (BossUIController.Instance != null)
        {
            BossUIController.Instance.ShowBossUI();
        }

        // Smoothly increase HP from 0 to maxHp (Combat Intro fill-up)
        float fillDuration = 1.5f; // Fills up over 1.5 seconds
        float timer = 0f;
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        while (timer < fillDuration)
        {
            timer += Time.deltaTime;
            hp = Mathf.Lerp(0f, maxHp, timer / fillDuration);
            yield return null; // Wait for the next frame
        }
        hp = maxHp; // Ensure it ends exactly at maxHp
    }

    private void Update()
    {
        // check starting state for no damage recieve
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("BossCoffinState") && !stateInfo.IsName("BossCoffinShake") && !stateInfo.IsName("BossAwake") && isSleeping)
        {
            isSleeping = false;
        }

        // if die do nothing
        if (isDie || isSleeping) return;

        if (hp <= 0)
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                StartCoroutine(FadeOutAudio(2f)); // Smoothly fade out music over 2 seconds
            }

            if (animator != null)
            {
                // Reset all action parameters to avoid visual conflicts
                animator.SetBool("ChargeStabAttack", false);
                animator.SetBool("StabAttack", false);
                animator.SetBool("JumpUp", false);
                animator.SetBool("JumpDown", false);
                animator.SetBool("IsMoving", false);

                // Force play the death state directly to bypass missing Animator transitions
                animator.Play("Die");
                animator.SetTrigger("Die"); // Fallback trigger
            }

            isDie = true;

            // Trigger the jumpscare sequence!
            if (JumpScare.Instance != null)
            {
                JumpScare.Instance.TriggerJumpScare();
            }
        }

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        // sweep attack trigger
        if (playerDistance < sweepingAttackTriggerRange && sweepingAttackColdownRemain >= sweepingAttackColdown && !isChargeStab && !isDashing && !isJumping) {
            animator.SetTrigger("SweepAttack");
            sweepingAttackColdownRemain = 0;
            attackTimer -= sweepingAttackTimer;
            StartCoroutine(DealSweepDamage()); // Trigger sweep damage check
            
            // Play sweeping attack sound!
            if (audioSource != null && sweepSound != null)
            {
                audioSource.PlayOneShot(sweepSound);
            }
        } else {
            sweepingAttackColdownRemain += Time.deltaTime;
        }

        // stab attack trigger
        if (playerDistance >= sweepingAttackTriggerRange && playerDistance < stabAttackTriggerRange && stabAttackColdownRemain >= stabAttackColdown && attackTimer >= 0f && !isChargeStab && !isDashing && !isJumping) {
            animator.SetBool("ChargeStabAttack", true);
            stabAttackColdownRemain = 0;
            isChargeStab = true;
            stabAttackChargeTimer = 0f; // Reset timer when starting to charge
        } else {
            stabAttackColdownRemain += Time.deltaTime;
        }

        // jump attack trigger
        if (playerDistance >= stabAttackTriggerRange && playerDistance < jumpAttackTriggerRange && jumpAttackColdownRemain >= jumpAttackColdown && attackTimer >= 0f && !isChargeStab && !isDashing && !isJumping) {
            animator.SetBool("JumpUp", true);
            jumpAttackColdownRemain = 0f;
            isJumping = true;
            if (myCollider != null) myCollider.enabled = false; // Disable collider to fly through obstacles/walls
            jumpPhase = 1;
            jumpTimer = 0f;
            currentJumpHeight = 0f; // Reset current height
            jumpStartPosition = transform.position;
            if (player != null)
            {
                jumpTargetPosition = player.transform.position;
            }

            // Play jump up sound!
            if (audioSource != null && jumpUpSound != null)
            {
                audioSource.PlayOneShot(jumpUpSound);
            }
        } else {
            jumpAttackColdownRemain += Time.deltaTime;
        }

        if (isChargeStab)
        {
            stabAttackChargeTimer += Time.deltaTime;
            if (stabAttackChargeTimer >= stabAttackChargeTime)
            {
                animator.SetBool("ChargeStabAttack", false);
                animator.SetBool("StabAttack", true);
                isChargeStab = false;
                stabAttackChargeTimer = 0f;

                // Record player position at this moment as our dash target
                if (player != null)
                {
                    isDashing = true;
                    dashTargetPosition = player.transform.position;
                    dashTimer = 0f; // Reset safety timer when starting the dash
                    hasHitThisDash = false; // Reset hit flag for this new dash

                    // Enable the stab dash trigger hitbox!
                    if (stabHitbox != null)
                    {
                        stabHitbox.SetDamage(stabbingDamage);
                        stabHitbox.EnableHitbox();
                    }

                    // Play stab dash sound!
                    if (audioSource != null && stabSound != null)
                    {
                        audioSource.PlayOneShot(stabSound);
                    }
                }
            }
        }

        // Handle active stab dash
        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            float distToTarget = Vector3.Distance(transform.position, dashTargetPosition);

            if (distToTarget <= 0.2f || dashTimer >= maxDashDuration)
            {
                // Reached target or timeout! Stop dashing
                isDashing = false;
                rb2d.velocity = Vector2.zero;
                animator.SetBool("StabAttack", false); // Return to idle/movement

                // Disable the stab dash trigger hitbox!
                if (stabHitbox != null)
                {
                    stabHitbox.DisableHitbox();
                }
            }
            else
            {
                // Dash forward at stabAttackSpeed relative to normal speed
                Vector2 dir = (dashTargetPosition - transform.position).normalized;
                rb2d.velocity = dir * (speed * stabAttackSpeed);
            }
        }
        // Handle active jump slam attack
        else if (isJumping)
        {
            rb2d.velocity = Vector2.zero; // Stop standard physics velocity during jump

            if (jumpPhase == 1) // Phase 1: Jump Up (Rising)
            {
                // Move parent horizontally and vertically on ground towards the target coordinates
                Vector2 currentPos2D = transform.position;
                Vector2 targetPos2D = jumpTargetPosition;
                Vector2 nextPos2D = Vector2.MoveTowards(currentPos2D, targetPos2D, speed * jumpHorizontalSpeed * Time.deltaTime);
                transform.position = new Vector3(nextPos2D.x, nextPos2D.y, transform.position.z);
                
                // Rise visual Y height linearly up to jumpHeight
                currentJumpHeight = Mathf.MoveTowards(currentJumpHeight, jumpHeight, jumpRiseSpeed * Time.deltaTime);
                animator.transform.localPosition = new Vector3(0f, currentJumpHeight, 0f);

                // End phase when we reach the target X axis and the max height
                if (Mathf.Abs(transform.position.x - jumpTargetPosition.x) <= 0.1f && currentJumpHeight >= jumpHeight)
                {
                    // Transition to Phase 2: Landing
                    jumpPhase = 2;
                    animator.SetBool("JumpUp", false);
                    animator.SetBool("JumpDown", true);

                    // Play jump slam down sound!
                    if (audioSource != null && jumpDownSound != null)
                    {
                        audioSource.PlayOneShot(jumpDownSound);
                    }
                }
            }
            else if (jumpPhase == 2) // Phase 2: Jump Down (Landing)
            {
                // Move visual animator child back down to 0 at falling speed
                currentJumpHeight = Mathf.MoveTowards(currentJumpHeight, 0f, jumpFallSpeed * Time.deltaTime);
                animator.transform.localPosition = new Vector3(0f, currentJumpHeight, 0f);

                if (currentJumpHeight <= 0f)
                {
                    // Trigger slam hit detection using the jump slam hitbox!
                    StartCoroutine(TriggerJumpSlamHitbox());

                    // Play the landing slam animation!
                    if (animator != null && !string.IsNullOrEmpty(jumpImpactTriggerName))
                    {
                        animator.SetTrigger(jumpImpactTriggerName);
                    }

                    // Landed! Reset states
                    isJumping = false;
                    if (myCollider != null) myCollider.enabled = true; // Re-enable collider on landing
                    jumpPhase = 0;
                    animator.SetBool("JumpDown", false);
                    animator.transform.localPosition = Vector3.zero; // Ensure local offset is exactly reset

                    // Delay 2 seconds before doing other actions/attacks
                    attackTimer = -2f;
                }
            }
        }
        else if (playerDistance > stoppingDistance && attackTimer >= 0f && !isChargeStab && !isJumping) {
            // move toward player (normal chase)
            Vector2 direction = (player.transform.position - transform.position).normalized;
            rb2d.velocity = direction * speed;
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);
            animator.SetBool("IsMoving", true);
        } else {
            rb2d.velocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            attackTimer = Mathf.Clamp(attackTimer + Time.deltaTime, -100f, 0f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDie || isSleeping) return;
        
        hp -= damage;
    }

    private IEnumerator DealSweepDamage()
    {
        // Enable hitbox during the active swing window
        if (sweepHitbox != null)
        {
            sweepHitbox.SetDamage(sweepingAttackDamage);
            sweepHitbox.EnableHitbox();
        }
        yield return new WaitForSeconds(0.4f); // Duration the sweeping arc hitbox stays active
        if (sweepHitbox != null)
        {
            sweepHitbox.DisableHitbox();
        }
    }

    private IEnumerator TriggerJumpSlamHitbox()
    {
        if (jumpHitbox != null)
        {
            jumpHitbox.SetDamage(jumpAttackDamage);
            jumpHitbox.EnableHitbox();
        }
        yield return new WaitForSeconds(0.2f); // Slam active duration
        if (jumpHitbox != null)
        {
            jumpHitbox.DisableHitbox();
        }
    }

    private IEnumerator FadeOutAudio(float duration)
    {
        if (audioSource == null) yield break;

        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Reset volume back for future playbacks
    }

    public float GetHp() => hp;
    public float GetMaxHp() => maxHp;
    public bool IsAwake() => !isSleeping;
    public bool IsDead() => isDie;

}

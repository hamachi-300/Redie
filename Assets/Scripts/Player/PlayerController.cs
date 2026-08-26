using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravity = 30f;

    [Header("Roll Settings")]
    [SerializeField] private float rollSpeed = 10f;
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private float rollAnimationFrame = 30f;

    [Header("Weapon Visual Settings")]
    [SerializeField] private SpriteRenderer weaponVisualRenderer;

    [Header("Stamina Costs")]
    [SerializeField] private float staminaCostRoll = 30f;
    [SerializeField] private float staminaCostJump = 10f;
    [SerializeField] private float staminaCostLightAttack = 10f;
    [SerializeField] private float staminaCostHeavyAttack = 20f;

    [Header("Heavy Attack Settings")]
    [SerializeField] private float heavyAttackHoldTime = 1f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip rollSound;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip runSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip deathSoundScreen;
    [SerializeField] private float walkStepInterval = 0.4f; // ระยะห่างก้าวตอนเดิน (วินาที)
    [SerializeField] private float runStepInterval = 0.25f; // ระยะห่างก้าวตอนวิ่ง (วินาที)

    [Header("Player Visual Root")]
    [SerializeField] private Transform playerVisual;

    [Header("Weapon Slot")]
    [SerializeField] private WeaponData equippedWeapon;

    // State Variables
    private float currentSpeed;
    private float height = 0f;
    private float verticalVelocity = 0f;
    private float remainHAHT;
    private float stepTimer = 0f;
    private bool isGrounded = true;
    private bool isChargingHeavy = false;
    private bool isRolling = false;
    private bool isDie = false;
    private RuntimeAnimatorController baseAnimatorController;

    private Vector2 rollDirection;
    private Rigidbody2D rb2d;
    private Animator animator;

    // Component Reference to separated stats
    private PlayerStats stats;

    // Public Getters (Wrappers targeting PlayerStats to ensure other scripts don't break)
    public float CurrentStamina => stats != null ? stats.CurrentStamina : 0f;
    public float MaxStamina => stats != null ? stats.MaxStamina : 100f;
    public float CurrentHealth => stats != null ? stats.CurrentHealth : 0f;
    public float MaxHealth => stats != null ? stats.MaxHealth : 100f;
    
    public WeaponData EquippedWeapon => equippedWeapon;
    public Animator PlayerAnimator => animator;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        
        // Find AudioSource component if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Find or dynamically add the stats component to prevent null reference issues
        stats = GetComponent<PlayerStats>();
        if (stats == null)
        {
            stats = gameObject.AddComponent<PlayerStats>();
        }

        // Save default animation controller
        if (animator != null)
        {
            baseAnimatorController = animator.runtimeAnimatorController;
        }

        if (rb2d == null) 
        {
            Debug.LogError("Rigidbody2D component not found on the player GameObject");
        } 
        else 
        {
            rb2d.gravityScale = 0f;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Initialize state variables
        currentSpeed = walkSpeed;

        // Start facing south (down)
        if (animator != null)
        {
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", -1f); 
        }

        if (equippedWeapon != null)
        {
            EquipWeapon(equippedWeapon);
        }

        // Teleport Spawn Position check
        if (!string.IsNullOrEmpty(SceneTransition.nextSpawnPointName))
        {
            GameObject spawnPoint = GameObject.Find(SceneTransition.nextSpawnPointName);
            if (spawnPoint != null)
            {
                rb2d.position = spawnPoint.transform.position;
                transform.position = spawnPoint.transform.position;
                Debug.Log("Player spawned at custom point: " + SceneTransition.nextSpawnPointName);
            }
            else
            {
                Debug.LogWarning("Spawn point '" + SceneTransition.nextSpawnPointName + "' not found in scene!");
            }
            SceneTransition.nextSpawnPointName = null; // Clear to prevent double spawn
        }
    }

    void Update()
    {
        // Check if menu window is open or player is dead
        if (MenuWindowUI.IsOpen || isDie) return;

        // Get WASD/Arrow keys input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveX, moveY);
        bool isMoving = (moveDirection.sqrMagnitude > 0.01f);
        bool isSprinting = false;

        // Movement
        if (!isRolling)
        {
            bool wantsToSprint = Input.GetKey(KeyCode.LeftShift);

            if (isMoving) 
            {
                animator.SetBool("IsMoving", true);
                animator.SetFloat("MoveX", moveDirection.x);
                animator.SetFloat("MoveY", moveDirection.y);
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }

            // Sprint only if moving, pressing sprint key, and have enough stamina in stats
            bool hasStaminaToSprint = (stats.CurrentStamina > stats.SprintStaminaDrainRate * Time.deltaTime);
            if (wantsToSprint && isMoving && hasStaminaToSprint && isGrounded)
            {
                isSprinting = true;
                currentSpeed = runSpeed;
                animator.SetFloat("WalkAnimSpeed", 1f);
                stats.DrainSprintStamina(); // Drain stamina inside stats
            }
            else
            {
                currentSpeed = walkSpeed;
                animator.SetFloat("WalkAnimSpeed", 0.5f);
            }
            
            if (moveDirection.sqrMagnitude > 1f) { moveDirection.Normalize(); }
            if (rb2d != null) { rb2d.velocity = moveDirection * currentSpeed; }
        }

        // Footstep Audio Handling
        HandleFootsteps(isMoving, isSprinting);

        // Regenerate stamina (only if not currently sprinting)
        bool isSprintingStaminaRegen = Input.GetKey(KeyCode.LeftShift) && isMoving && (stats.CurrentStamina > 0f);
        stats.RegenerateStamina(isSprintingStaminaRegen);

        // Get direction of player facing
        float facingX = animator.GetFloat("MoveX");
        float facingY = animator.GetFloat("MoveY");

        // Heavy attack charge conditions
        if (Input.GetMouseButtonDown(0)) 
        { 
            remainHAHT = heavyAttackHoldTime; 
            isChargingHeavy = false;
        }
        if (Input.GetMouseButton(0)) 
        { 
            remainHAHT -= Time.deltaTime; 

            // Stance Charge pose after holding 1 second
            if (remainHAHT <= 0f && !isChargingHeavy)
            {
                isChargingHeavy = true;
                animator.SetBool("IsChargingHeavy", true);
            }
        }

        // Perform attack on mouse release
        if (Input.GetMouseButtonUp(0))
        {
            if (!isGrounded)
            {
                // Airborne Attack
                if (stats.ConsumeStamina(staminaCostLightAttack))
                {
                    Debug.Log("Mid-Air Jump Attack!");
                    AttackColliderControl hitbox = weaponVisualRenderer.GetComponent<AttackColliderControl>();
                    if (hitbox != null) hitbox.ResetHitbox();
                    animator.SetTrigger("LightAttack");
                    verticalVelocity -= 8f; 
                    PlayAttackSound();
                }
                if (isChargingHeavy)
                {
                    animator.SetBool("IsChargingHeavy", false);
                    isChargingHeavy = false;
                }
            }
            else 
            {
                if (isChargingHeavy)
                {
                    // HEAVY ATTACK RELEASE SLAM
                    if (stats.ConsumeStamina(staminaCostHeavyAttack))
                    {
                        Debug.Log("Heavy Attack Release Slam!");
                        AttackColliderControl hitbox = weaponVisualRenderer.GetComponent<AttackColliderControl>();
                        if (hitbox != null) hitbox.ResetHitbox();

                        animator.SetBool("IsChargingHeavy", false);
                        animator.SetTrigger("HeavyAttack");
                        PlayAttackSound();
                    }
                    else
                    {
                        Debug.Log("Not enough stamina for heavy attack!");
                        animator.SetBool("IsChargingHeavy", false);
                    }
                    isChargingHeavy = false;
                }
                else
                {
                    // LIGHT ATTACK (Released early)
                    if (stats.ConsumeStamina(staminaCostLightAttack))
                    {
                        Debug.Log("Light Attack!");
                        AttackColliderControl hitbox = weaponVisualRenderer.GetComponent<AttackColliderControl>();
                        if (hitbox != null) hitbox.ResetHitbox();

                        animator.SetTrigger("LightAttack");
                        PlayAttackSound();
                    }
                    else
                    {
                        Debug.Log("Not enough stamina for light attack!");
                    }
                }
            }
        }

        // Jumping 
        if (Input.GetKeyDown(KeyCode.F) && isGrounded && stats.ConsumeStamina(staminaCostJump))
        {
            verticalVelocity = jumpForce;
            isGrounded = false;
            float airTime = (2f * jumpForce) / gravity;
            float clipLength = 7f / 60f;

            animator.SetFloat("JumpAnimSpeed", clipLength / airTime);
            animator.SetBool("IsJumping", true);
            PlayJumpSound();
        }

        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            height += verticalVelocity * Time.deltaTime;
            
            // Land on ground
            if (height <= 0f)
            {
                height = 0f;
                verticalVelocity = 0f;
                isGrounded = true;
                animator.SetBool("IsJumping", false);
            }
        }

        // Rolling (Dodge Roll)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && stats.ConsumeStamina(staminaCostRoll) && !isRolling)
        {
            float rollClipLength = rollAnimationFrame / 60f;
            animator.SetFloat("RollAnimSpeed", rollClipLength / rollDuration);

            rollDirection = new Vector2(facingX, facingY).normalized;

            if (rollDirection == Vector2.zero) rollDirection = Vector2.down;

            animator.SetTrigger("Roll");
            PlayRollSound();
            StartCoroutine(PerformRoll());
        }
    }

    private void HandleFootsteps(bool isMoving, bool isSprinting)
    {
        if (isMoving && isGrounded && !isRolling)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                AudioClip clipToPlay = isSprinting ? (runSound != null ? runSound : walkSound) : walkSound;
                if (audioSource != null && clipToPlay != null)
                {
                    // เปลี่ยนจาก PlayOneShot มาเป็นการใส่ clip แล้ว Play
                    // ช่วยให้ก้าวใหม่เล่นแทนที่ก้าวเก่าทันที ไม่ก้องหรือสะสมเสียง
                    audioSource.clip = clipToPlay;
                    audioSource.Play();
                }
                
                stepTimer = isSprinting ? runStepInterval : walkStepInterval;
            }
        }
        else
        {
            // หยุดเล่นเสียงเฉพาะเมื่อเสียงที่เล่นอยู่ปัจจุบันคือเสียงเดินหรือวิ่ง
            if (audioSource != null && audioSource.isPlaying)
            {
                if (audioSource.clip == walkSound || audioSource.clip == runSound)
                {
                    audioSource.Stop();
                }
            }
            stepTimer = 0f;
        }
    }

    public void PlayAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void PlayJumpSound()
    {
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    public void PlayRollSound()
    {
        if (audioSource != null && rollSound != null)
        {
            audioSource.PlayOneShot(rollSound);
        }
    }

    public void PlayDeathSound()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.volume = 1.0f; // Force AudioSource volume to 100%
            audioSource.PlayOneShot(deathSound, 1.0f); // Force PlayOneShot to play at max volume
        }
    }

    public void PlayDeathSoundScreen()
    {
        if (audioSource != null && deathSoundScreen != null)
        {
            audioSource.volume = 1.0f; // Force AudioSource volume to 100%
            audioSource.PlayOneShot(deathSoundScreen, 1.0f); // Force PlayOneShot to play at max volume
        }
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        if (newWeapon == null) return;

        // TOGGLE: If you try to equip the same weapon that is already active, unequip it!
        if (equippedWeapon == newWeapon)
        {
            UnequipWeapon();
            return; 
        }

        equippedWeapon = newWeapon;

        // Swap the sprite
        if (weaponVisualRenderer != null)
        {
            weaponVisualRenderer.sprite = newWeapon.itemSprite;
            weaponVisualRenderer.transform.localScale = newWeapon.localScale;
        }

        // Swap the Animator Override Controller
        if (animator != null && newWeapon.animatorOverride != null)
        {
            animator.runtimeAnimatorController = newWeapon.animatorOverride;
        }

        // Update stats costs
        staminaCostLightAttack = newWeapon.staminaCostLight;
        staminaCostHeavyAttack = newWeapon.staminaCostHeavy;

        Debug.Log("Equipped: " + newWeapon.itemName);
    }

    public void UnequipWeapon()
    {
        equippedWeapon = null;

        if (weaponVisualRenderer != null)
        {
            weaponVisualRenderer.sprite = null;
        }

        if (animator != null && baseAnimatorController != null)
        {
            animator.runtimeAnimatorController = baseAnimatorController;
        }

        // Reset costs to unarmed defaults
        staminaCostLightAttack = 10f; 
        staminaCostHeavyAttack = 20f;
        Debug.Log("Weapon Unequipped.");
    }

    public void Heal(float amount)
    {
        stats.Heal(amount);
    }

    public void TakeDamage(float damage, bool isInvincible = false)
    {
        stats.TakeDamage(damage, isInvincible);
    }

    public void Die()
    {
        isDie = true;
        animator.SetTrigger("Die");
        PlayDeathSound();
        PlayDeathSoundScreen();
        
        // Show the Game Over screen if it exists in the scene
        if (GameOverUI.Instance != null)
        {
            GameOverUI.Instance.ShowGameOverScreen();
        }
    }

    private IEnumerator PerformRoll()
    {
        isRolling = true;
        float timer = 0f;
        while (timer < rollDuration)
        {
            timer += Time.deltaTime;
            
            // Push Rigidbody2D forward in facing direction!
            if (rb2d != null)
            {
                rb2d.velocity = rollDirection * rollSpeed;
            }
            yield return null;
        }
        isRolling = false;
    }

    void LateUpdate()
    {
        // Sync visual position and height (jump offset)
        if (playerVisual != null)
        {
            playerVisual.localPosition = new Vector3(0, height, 0);
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.offset = new Vector2(col.offset.x, height);
            }
        }
    }
}
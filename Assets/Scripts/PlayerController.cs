using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;

    [Header("Status Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float maxHealth = 100f;

    [Header("Attack Settings")]
    [SerializeField] private float heavyAttackHoldTime =  1f;
    [SerializeField] private float staminaCostLightAttack = 10f;
    [SerializeField] private float staminaCostHeavyAttack = 20f;

    [Header("Weapon Settings")]
    [SerializeField] private SpriteRenderer weaponVisualRenderer;

    [Header("Equipped Weapon")]
    [SerializeField] private WeaponData equippedWeapon;

    [Header("Jump Settings")]
    [SerializeField] private Transform playerVisual;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravity = 30f;
    [SerializeField] private float staminaCostJump = 10f;

    [Header("Roll Settings")]
    [SerializeField] private float rollSpeed = 15f;
    [SerializeField] private float rollDuration = 0.4f;
    [SerializeField] private float staminaCostRoll = 30f;

    private float remainHAHT;
    private float currentStamina;
    private float currentSpeed;
    private float height = 0f;
    private float verticalVelocity = 0f;
    private bool isGrounded = true;
    private bool isChargingHeavy = false;
    private bool isRolling = false;
    private float currentHealth;
    
    private Vector2 rollDirection;
    private Rigidbody2D rb2d;
    private Animator animator;

    // getters
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        if (rb2d == null) {
            Debug.LogError("Rigidbody2D component not found on the player GameObject");
        } else {
            // make player not rotate and no gravity
            rb2d.gravityScale = 0f;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // define startup status
        currentStamina = maxStamina;
        currentSpeed = walkSpeed;
        currentHealth = maxHealth;

        // start face direction south
        animator.SetFloat("MoveX", 0f);
        animator.SetFloat("MoveY", -1f); 

        if (equippedWeapon != null)
        {
            EquipWeapon(equippedWeapon);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // check is menu window is opening
        if (MenuWindowUI.IsOpen) return;

        // Get WASD/Arrow keys input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveX, moveY);

        // movement
        if (!isRolling)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = runSpeed;
                animator.SetFloat("WalkAnimSpeed", 1f);
            }
            else
            {
                currentSpeed = walkSpeed;
                animator.SetFloat("WalkAnimSpeed", 0.5f);
            }
            if (moveDirection.sqrMagnitude > 1f){ moveDirection.Normalize(); }
            if (rb2d != null) { rb2d.velocity = moveDirection * currentSpeed; }

            // animation blend tree
            bool isMoving = (moveDirection.sqrMagnitude > 0.01f);
            animator.SetBool("IsMoving", isMoving);

            if (isMoving)
            {
                animator.SetFloat("MoveX", moveDirection.x);
                animator.SetFloat("MoveY", moveDirection.y);
            }
        }

        // regenerate stamina
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }

        // get direction of player facing
        float facingX = animator.GetFloat("MoveX");
        float facingY = animator.GetFloat("MoveY");

        // attack type condition
        if (Input.GetMouseButtonDown(0)) { remainHAHT = heavyAttackHoldTime; }
        if (Input.GetMouseButton(0)) { remainHAHT -= Time.deltaTime; }

        // reset heavy attack remain time when mouse down
        if (Input.GetMouseButtonDown(0)) 
        { 
            remainHAHT = heavyAttackHoldTime; 
            isChargingHeavy = false;
        }

        // held mouse for decrease remain heavy attack time and  enter charge stance
        if (Input.GetMouseButton(0)) 
        { 
            remainHAHT -= Time.deltaTime; 

            // Once held past 1 second -> Play Charge Pose!
            if (remainHAHT <= 0f && !isChargingHeavy)
            {
                isChargingHeavy = true;
                animator.SetBool("IsChargingHeavy", true);
            }
        }

        // mouse released -> perform attack
        if (Input.GetMouseButtonUp(0))
        {
            // Airborne attack
            if (!isGrounded)
            {
                if (currentStamina >= staminaCostLightAttack)
                {
                    Debug.Log("Mid-Air Jump Attack!");
                    AttackColliderControl hitbox = weaponVisualRenderer.GetComponent<AttackColliderControl>();
                    if (hitbox != null) hitbox.ResetHitbox();
                    animator.SetTrigger("LightAttack");
                    verticalVelocity -= 8f; 
                    currentStamina -= staminaCostLightAttack;
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
                    if (currentStamina >= staminaCostHeavyAttack)
                    {
                        Debug.Log("Heavy Attack Release Slam!");
                        AttackColliderControl hitbox = weaponVisualRenderer.GetComponent<AttackColliderControl>();
                        if (hitbox != null) hitbox.ResetHitbox();

                        animator.SetBool("IsChargingHeavy", false);
                        animator.SetTrigger("HeavyAttack");
                        currentStamina -= staminaCostHeavyAttack;
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
                    if (currentStamina >= staminaCostLightAttack)
                    {
                        Debug.Log("Light Attack!");
                        AttackColliderControl hitbox = weaponVisualRenderer.GetComponent<AttackColliderControl>();
                        if (hitbox != null) hitbox.ResetHitbox();

                        animator.SetTrigger("LightAttack");
                        currentStamina -= staminaCostLightAttack;
                    }
                    else
                    {
                        Debug.Log("Not enough stamina for light attack!");
                    }
                }
            }
        }


        // jumping 
        if (Input.GetKeyDown(KeyCode.F) && isGrounded && currentStamina >= staminaCostJump)
        {
            verticalVelocity = jumpForce;
            isGrounded = false;
            float airTime = (2f * jumpForce) / gravity;
            float clipLength = 7f / 60f;

            animator.SetFloat("JumpAnimSpeed", clipLength / airTime);
            animator.SetBool("IsJumping", true);
            currentStamina -= staminaCostJump;
        }

        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            height += verticalVelocity * Time.deltaTime;
            // on land on ground
            if (height <= 0f)
            {
                height = 0f;
                verticalVelocity = 0f;
                isGrounded = true;
                animator.SetBool("IsJumping", false);
            }
        }

        // rolling 
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && currentStamina >= staminaCostRoll)
        {
            rollDirection = new Vector2(facingX, facingY).normalized;

            // if stay still roll south
            if (rollDirection == Vector2.zero) rollDirection = Vector2.down;

            animator.SetTrigger("Roll");
            currentStamina -= staminaCostRoll;
            StartCoroutine(PerformRoll());
        }
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        if (newWeapon == null) return;

        equippedWeapon = newWeapon;

        // swap the sprite
        if (weaponVisualRenderer != null)
        {
            weaponVisualRenderer.sprite = newWeapon.itemSprite;
            
            // Apply custom scale for the weapon (e.g. Greatsword is larger)
            weaponVisualRenderer.transform.localScale = newWeapon.localScale;
        }

        // swap the Animator Override Controller
        if (animator != null && newWeapon.animatorOverride != null)
        {
            animator.runtimeAnimatorController = newWeapon.animatorOverride;
        }

        // update stats
        staminaCostLightAttack = newWeapon.staminaCostLight;
        staminaCostHeavyAttack = newWeapon.staminaCostHeavy;
        
        Debug.Log("Equipped: " + newWeapon.itemName);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log("Healed! Current HP: " + currentHealth + "/" + maxHealth);
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
        if (!isGrounded && playerVisual != null)
        {
            playerVisual.localPosition = new Vector3(0, height, 0);
        }
    }

}

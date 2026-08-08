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
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = 15f;
    [SerializeField] private float staminaCostJump = 10f;

    private float remainHAHT;
    private float currentStamina;
    private float currentSpeed;
    private float height = 0f;
    private float verticalVelocity = 0f;
    private bool isGrounded = true;

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
        // Get WASD/Arrow keys input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveX, moveY);

        // movement
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

        // light and heavy attack logic
        if (Input.GetMouseButtonUp(0))
        {
            if (remainHAHT > 0)
            {
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
            else
            {
                if (currentStamina >= staminaCostHeavyAttack)
                {
                    Debug.Log("Heavy Attack!");
                    AttackColliderControl hitbox = weaponVisualRenderer.GetComponent<AttackColliderControl>();
                    if (hitbox != null) hitbox.ResetHitbox();
                    animator.SetTrigger("HeavyAttack");
                    currentStamina -= staminaCostHeavyAttack;
                }
                else
                {
                    Debug.Log("Not enough stamina for heavy attack!");
                }
            }
        }

        // jumping 
        if (Input.GetKeyDown(KeyCode.F) && isGrounded && currentStamina >= staminaCostJump)
        {
            verticalVelocity = jumpForce;
            isGrounded = false;
            animator.SetBool("IsJumping", true);
            currentStamina -= staminaCostJump;
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
            // Apply visual height offset
            if (playerVisual != null)
            {
                playerVisual.localPosition = new Vector3(0, height, 0);
            }
        }
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        if (newWeapon == null) return;

        equippedWeapon = newWeapon;

        // swap the sprite
        if (weaponVisualRenderer != null)
        {
            weaponVisualRenderer.sprite = newWeapon.weaponSprite;
            
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
        
        Debug.Log("Equipped: " + newWeapon.weaponName);
    }
}

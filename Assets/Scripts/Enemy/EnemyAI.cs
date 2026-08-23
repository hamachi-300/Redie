using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float signRange = 10f;

    [Header("Default Facing Direction")]
    [SerializeField] private Vector2 defaultFacingDirection = Vector2.down;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolWaypoints;
    [SerializeField] private float patrolSpeed = 1.5f;

    [Header("Vision Settings")]
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float attackDelayTime = 0.3f;

    [Header("Attack Hitbox Reference")]
    [SerializeField] private EnemyAttackHitbox attackHitbox;

    private float nextAttackTime = 0f;
    private bool isAttacking = false;
    private bool isFoundPlayer = false;
    private int currentWaypointIndex = 0;

    private Transform playerTransform;
    private Rigidbody2D rb2d;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        // Find the player automatically using their Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        // make enemy move fixed z rotate (prevent rotate while walk)
        if (rb2d != null)
        {
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation; 
            rb2d.gravityScale = 0f; // prevent slidedown when stopping
        }

        // === SET THE DEFAULT STARTING FACING DIRECTION ===
        if (animator != null)
        {
            animator.SetFloat("MoveX", defaultFacingDirection.x);
            animator.SetFloat("MoveY", defaultFacingDirection.y);
            animator.SetBool("IsMoving", false); // Start in Idle
        }
    }

    void Update()
    {
        // 1. Safety check
        if (playerTransform == null || rb2d == null) 
        {
            if (rb2d != null) rb2d.velocity = Vector2.zero;
            return;
        }

        // 2. Check distance
        float distance = Vector2.Distance(playerTransform.position, transform.position);
        bool isInSignRange = distance < signRange;
        bool hasLineOfSight = false;

        // 3. Shoot a raycast to check for solid obstacles between enemy and player
        if (isInSignRange)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distance, obstacleLayer);

            // If the ray hits nothing, the line of sight is clear!
            if (hit.collider == null)
            {
                hasLineOfSight = true;
            }
        }

        // 4. CHASE and ATTACK state if player is seen
        if (hasLineOfSight)
        {
            // double sign range when found player 
            if (!isFoundPlayer)
            {
                signRange = signRange * 2;
                isFoundPlayer = true;
            }

            Vector2 direction = (playerTransform.position - transform.position).normalized;

            // Attack Check
            if (distance <= attackRange)
            {
                // Stop walking
                rb2d.velocity = Vector2.zero;
                if (animator != null)
                {
                    animator.SetBool("IsMoving", false);
                    animator.SetFloat("MoveX", direction.x);
                    animator.SetFloat("MoveY", direction.y);
                }

                // If cooldown has passed, start the attack!
                if (!isAttacking && Time.time >= nextAttackTime)
                {
                    StartCoroutine(PerformAttack());
                }
            }
            else if (!isAttacking) // Only walk if we are not currently attacking
            {
                // Move the Rigidbody toward the player
                rb2d.velocity = direction * speed;

                // animation
                if (animator != null)
                {
                    bool isMoving = (rb2d.velocity.sqrMagnitude > 0.01f);
                    animator.SetBool("IsMoving", isMoving);
                    if (isMoving)
                    {
                        animator.SetFloat("MoveX", direction.x);
                        animator.SetFloat("MoveY", direction.y);
                    }
                }
            }
        }
        else
        {
            // 5. PATROL / STAND STILL state if player is out of sight
            rb2d.velocity = Vector2.zero;

            if (isFoundPlayer)
            {
                signRange = signRange / 2;
                isFoundPlayer = false;
            }

            // Only patrol if we have waypoints assigned
            if (patrolWaypoints != null && patrolWaypoints.Length > 0)
            {
                Transform targetWaypoint = patrolWaypoints[currentWaypointIndex];
                Vector2 direction = (targetWaypoint.position - transform.position).normalized;

                if (!isAttacking) // Only move if we are not locked in an attack animation
                {
                    rb2d.velocity = direction * patrolSpeed;

                    if (animator != null)
                    {
                        animator.SetBool("IsMoving", true);
                        animator.SetFloat("MoveX", direction.x);
                        animator.SetFloat("MoveY", direction.y);
                    }
                }

                // Check if we reached the waypoint
                if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.2f)
                {
                    currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
                }
            }
            else
            {
                // Stand still if no waypoints assigned
                rb2d.velocity = Vector2.zero;
                if (animator != null)
                {
                    animator.SetBool("IsMoving", false);
                }
            }
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        // 1. Play attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack"); 
        }

        // 2. Stop movement during swing
        rb2d.velocity = Vector2.zero;
        if (animator != null) animator.SetBool("IsMoving", false);

        // 3. Wait for the telegraph phase (wind-up)
        yield return new WaitForSeconds(attackDelayTime);

        // 4. ACTIVATE THE HITBOX during the swing!
        if (attackHitbox != null)
        {
            attackHitbox.SetDamage(attackDamage);
            attackHitbox.EnableHitbox();
        }

        // 5. Keep the hitbox active for the duration of the swing (e.g. 0.15 seconds)
        yield return new WaitForSeconds(0.15f);

        // 6. DEACTIVATE the hitbox immediately after the swing is over
        if (attackHitbox != null)
        {
            attackHitbox.DisableHitbox();
        }

        // 7. Wait for the recovery animation to finish
        yield return new WaitForSeconds(0.25f); 
        isAttacking = false;
    }
}

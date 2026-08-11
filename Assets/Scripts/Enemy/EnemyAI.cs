using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] private float speed = 3f;

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
        }
    }

    // Update is called once per frame
    void Update()
    {
        // make enemy move forward to player
        if (playerTransform != null && rb2d != null)
        {
            // Calculate the direction from the Enemy to the Player
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            // Move the Rigidbody toward the player
            rb2d.velocity = direction * speed;

            // animation
            if (animator != null)
            {
                // Check if the enemy is moving
                bool isMoving = (rb2d.velocity.sqrMagnitude > 0.01f);
                animator.SetBool("IsMoving", isMoving);
                if (isMoving)
                {
                    // Send the movement direction to the Blend Tree
                    animator.SetFloat("MoveX", direction.x);
                    animator.SetFloat("MoveY", direction.y);
                }
            }
        }
    }
}

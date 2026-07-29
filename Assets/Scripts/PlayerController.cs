using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed of the player movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody2D component if it exists
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get WASD/Arrow keys input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Combine inputs into a direction vector
        Vector2 moveDirection = new Vector2(moveX, moveY);

        // Normalize direction vector to prevent faster diagonal movement
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Apply movement
        if (rb2d != null)
        {
            rb2d.velocity = moveDirection * moveSpeed;
        }
        else
        {
            // Fallback for simple transform translation if there is no Rigidbody2D
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}

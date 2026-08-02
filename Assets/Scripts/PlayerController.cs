using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float heavyAttackHoldTime =  1f;

    private Rigidbody2D rb2d;
    private float remainHAHT;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null) {
            Debug.LogError("Rigidbody2D component not found on the player GameObject");
        } else {
            // make player not rotate and no gravity
            rb2d.gravityScale = 0f;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
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
        if (moveDirection.sqrMagnitude > 1f){ moveDirection.Normalize(); }
        if (rb2d != null) { rb2d.velocity = moveDirection * moveSpeed; }

        // attack type condition
        if (Input.GetMouseButtonDown(0)) { remainHAHT = heavyAttackHoldTime; }
        if (Input.GetMouseButton(0)) { remainHAHT -= Time.deltaTime; }

        // light and heavy attack logic
        if (Input.GetMouseButtonUp(0))
        {
            if (remainHAHT > 0)
            {
                Debug.Log("Light Attack!");
            }
            else
            {
                Debug.Log("Heavy Attack!");
            }
        }
    }
}

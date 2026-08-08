using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColliderControl : MonoBehaviour
{
    private PlayerController player;
    private List<Collider2D> alreadyHitTargets = new List<Collider2D>();

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void OnEnable()
    {
        alreadyHitTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (alreadyHitTargets.Contains(other)) return;
            alreadyHitTargets.Add(other);
            Debug.Log("Hit enemy: " + other.name);
        }
    }

    public void ResetHitbox()
    {
        alreadyHitTargets.Clear();
    }
}

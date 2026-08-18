using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))] 
public class HpUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;

    private Slider hpSlider;

    void Start()
    {
        hpSlider = GetComponent<Slider>();

        // Auto-detect player tag if not assigned in Inspector
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.GetComponent<PlayerController>();
            }
        }

        if (player != null && hpSlider != null)
        {
            hpSlider.maxValue = player.MaxHealth;
            hpSlider.value = player.CurrentHealth;
        }
        else
        {
            Debug.LogError("Player or hpSlider not found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (player != null && hpSlider != null)
        {
            hpSlider.value = player.CurrentHealth;
        }
    }
}

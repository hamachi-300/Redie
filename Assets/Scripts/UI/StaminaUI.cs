using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))] 
public class StaminaUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;

    private Slider staminaSlider;

    void Start()
    {
        staminaSlider = GetComponent<Slider>();

        // Auto-detect player tag if not assigned in Inspector
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.GetComponent<PlayerController>();
            }
        }

        if (player != null && staminaSlider != null)
        {
            staminaSlider.maxValue = player.MaxStamina;
            staminaSlider.value = player.CurrentStamina;
        }
        else
        {
            Debug.LogError("Player or StaminaSlider not found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (player != null && staminaSlider != null)
        {
            staminaSlider.value = player.CurrentStamina;
        }
    }
}

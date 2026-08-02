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

    // Start is called before the first frame update
    void Start()
    {
        staminaSlider = GetComponent<Slider>();

        if (player != null && staminaSlider != null)
        {
            staminaSlider.maxValue = player.MaxStamina;
            staminaSlider.value = player.CurrentStamina;
        } else {
            Debug.LogError("Player or StaminaSlider not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && staminaSlider != null)
        {
            staminaSlider.value = player.CurrentStamina;
        }
    }
}

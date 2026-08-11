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

    // Start is called before the first frame update
    void Start()
    {
        hpSlider = GetComponent<Slider>();

        if (player != null && hpSlider != null)
        {
            hpSlider.maxValue = player.MaxHealth;
            hpSlider.value = player.CurrentHealth;
        } else {
            Debug.LogError("Player or hpSlider not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && hpSlider != null)
        {
            hpSlider.value = player.CurrentHealth;
        }
    }
}

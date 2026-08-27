using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintUI : MonoBehaviour
{
    [Header("Hint Panel")]
    [SerializeField] private GameObject hintPanel;

    void Start()
    {
        if (hintPanel != null)
        {
            hintPanel.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHint();
        }
    }

    private void ToggleHint()
    {
        if (hintPanel == null) return;
        bool newState = !hintPanel.activeSelf;
        hintPanel.SetActive(newState);
    }
}
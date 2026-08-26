using System.Collections;
using UnityEngine;             
using UnityEngine.UI;     
using UnityEngine.SceneManagement;

public class SceneNameShow : MonoBehaviour
{
    [Header("Scene Name Setting")]
    [SerializeField] private string sceneName;
    [SerializeField] private float fadeInTime = 1.0f;
    [SerializeField] private float showTime = 3.0f;
    [SerializeField] private float fadeOutTime = 1.0f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sceneNameSound;

    // subscribe OnSceneLoaded in order to use OnSceneLoaded function
    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // OnSceneLoaded function trigger after load this scene
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        StartCoroutine(SceneNameTransition());
    }

    private IEnumerator SceneNameTransition(){
        // set up text 
        TMPro.TextMeshProUGUI textComponent = GetComponent<TMPro.TextMeshProUGUI>();
        Color originalColor = textComponent.color;
        textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        textComponent.text = "";

        // wait for 1 second
        yield return new WaitForSeconds(1.0f);

        // Play the sound after the 1-second delay
        if (audioSource != null && sceneNameSound != null)
        {
            audioSource.PlayOneShot(sceneNameSound);
        }

        textComponent.text = sceneName;
        // fade in 
        float timer = 0f;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeInTime);
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null; // Wait for the next frame
        }
        // ensure text will fully visible
        textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        yield return new WaitForSeconds(showTime);

        // fade out 
        timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (timer / fadeOutTime));
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null; // Wait for the next frame
        }

        // delete text and set text cannot see
        textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        textComponent.text = "";
    }
}
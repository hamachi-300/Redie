using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider progressBarSlider; // Optional: For standard UI Sliders (Min Value 0, Max Value 1)

    private void Start()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        yield return new WaitForSeconds(0.5f); // Optional: Give the screen a brief moment to show up

        // Load the target scene in the background
        AsyncOperation operation = SceneManager.LoadSceneAsync(LoadingManager.targetSceneName);
        operation.allowSceneActivation = false; // Prevent switching until it is 100% loaded

        while (!operation.isDone)
        {
            // operation.progress goes from 0 to 0.9. We convert it to a 0 to 1 value:
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            
            if (progressBarSlider != null)
            {
                progressBarSlider.value = progress;
            }

            // Once background loading reaches 90% (which means fully loaded), switch the scene
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}

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

        // 1. ตรวจสอบว่าได้กำหนดชื่อ Scene แล้วหรือยัง
        if (string.IsNullOrEmpty(LoadingManager.targetSceneName))
        {
            Debug.LogError("[LoadingSceneUI] ไม่พบชื่อ Scene ที่ต้องการโหลด! (targetSceneName เป็นค่าว่าง)");
            yield break; // หยุดการทำงานทันที ป้องกัน NullReferenceException
        }

        // Load the target scene in the background
        AsyncOperation operation = SceneManager.LoadSceneAsync(LoadingManager.targetSceneName);

        // 2. ตรวจสอบว่า AsyncOperation สร้างสำเร็จหรือไม่ (เช่น พิมพ์ชื่อ Scene ผิด หรือไม่ได้ใส่ใน Build Settings)
        if (operation == null)
        {
            Debug.LogError($"[LoadingSceneUI] ไม่สามารถโหลด Scene '{LoadingManager.targetSceneName}' ได้! กรุณาเช็คว่าใส่ชื่อถูกและเพิ่มใน Build Settings แล้วหรือยัง");
            yield break; // หยุดการทำงานทันที
        }

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
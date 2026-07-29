#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class AutoSaveOnPlay
{
    static AutoSaveOnPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        // Save scene and assets right before entering Play Mode
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            Debug.Log("AutoSave: Saving scenes and assets before entering Play Mode...");
            
            // Save all modified open scenes
            EditorSceneManager.SaveOpenScenes();
            
            // Save all modified project assets (prefabs, materials, etc.)
            AssetDatabase.SaveAssets();
        }
    }
}
#endif

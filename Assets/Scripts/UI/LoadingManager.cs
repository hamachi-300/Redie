using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadingManager
{
    // Remembers the name of the map scene we want to open
    public static string targetSceneName;

    public static void LoadScene(string sceneName)
    {
        targetSceneName = sceneName;
        
        // Load the Loading Screen scene
        SceneManager.LoadScene("LoadingScene");
    }
}

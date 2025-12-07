using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string firstLevelName = "Level_Village";

    public void StartGame()
    {
        if (ScreenFader.Instance != null)
            ScreenFader.Instance.FadeAndLoadScene(firstLevelName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(firstLevelName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

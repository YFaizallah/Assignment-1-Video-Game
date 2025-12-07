using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWinController : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        // Make sure UI can be clicked
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

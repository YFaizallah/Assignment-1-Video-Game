using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup;   // CanvasGroup on FadeImage
    public float fadeDuration = 1f;
    public TMPro.TextMeshProUGUI loadingText; // optional

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;      // ? start transparent

        if (loadingText != null)
            loadingText.gameObject.SetActive(false); // hide at start
    }

    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        if (loadingText != null)
            loadingText.gameObject.SetActive(true);

        // 1) Fade to black
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (fadeCanvasGroup != null)
                fadeCanvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        // 2) Load scene
        SceneManager.LoadScene(sceneName);
        yield return null; // wait 1 frame

        // 3) Fade back to clear
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (fadeCanvasGroup != null)
                fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        if (loadingText != null)
            loadingText.gameObject.SetActive(false);
    }
}

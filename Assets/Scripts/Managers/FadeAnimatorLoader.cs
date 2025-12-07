using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeAnimatorLoader : MonoBehaviour
{
    public static FadeAnimatorLoader Instance;

    [Header("Animator")]
    public Animator fadeAnimator;   // Animator on FadeImage
    public string fadeOutBoolName = "DoFadeOut";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void FadeOutAndLoad(string sceneName, float fadeDuration = 1f)
    {
        StartCoroutine(FadeOutAndLoadRoutine(sceneName, fadeDuration));
    }

    private IEnumerator FadeOutAndLoadRoutine(string sceneName, float fadeDuration)
    {
        if (fadeAnimator != null)
        {
            fadeAnimator.SetBool(fadeOutBoolName, true);
        }

        yield return new WaitForSeconds(fadeDuration);

        SceneManager.LoadScene(sceneName);
    }
}

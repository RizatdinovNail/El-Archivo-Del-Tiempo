using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    public CanvasGroup cg;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        cg.alpha = 0f;
    }

    // For loading a completely different Unity scene
    public void TransitionToScene(string sceneName, float fadeOutTime = 1f, float fadeInTime = 1f)
    {
        gameObject.SetActive(true);
        StartCoroutine(LoadSceneSequence(sceneName, fadeOutTime, fadeInTime));
    }

    // For fading while staying in the same scene
    public void TransitionToSameScene(Action onFadeComplete, float fadeOutTime = 1f, float fadeInTime = 1f)
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeSequence(onFadeComplete, fadeOutTime, fadeInTime));
    }

    // Coroutine for actual scene change
    private IEnumerator LoadSceneSequence(string sceneName, float fadeOutTime, float fadeInTime)
    {
        yield return FadeOutRoutine(fadeOutTime);

        SceneManager.LoadScene(sceneName);

        yield return null; // wait for scene to load

        yield return FadeInRoutine(fadeInTime);
    }

    // Coroutine for same-scene fade
    private IEnumerator FadeSequence(Action onFadeComplete, float fadeOutTime, float fadeInTime)
    {
        yield return FadeOutRoutine(fadeOutTime);

        // Invoke whatever logic you want after fade out
        onFadeComplete?.Invoke();

        yield return FadeInRoutine(fadeInTime);
    }

    public IEnumerator FadeOutRoutine(float duration)
    {
        cg.alpha = 0f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        cg.alpha = 1f;
    }

    public IEnumerator FadeInRoutine(float duration)
    {
        cg.alpha = 1f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }

        cg.alpha = 0f;
    }
}

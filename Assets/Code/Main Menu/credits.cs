using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class animation
{
    public Sprite backgroundSprite;
    public Sprite characterSprite;
    public string title;
    public string text;
    public string thanks;
}
public class credits : MonoBehaviour
{
    public GameObject background;
    public CanvasGroup overlay;
    public GameObject character;
    public List<animation> creditos;

    public TextMeshProUGUI title;
    public TextMeshProUGUI text;
    public TextMeshProUGUI thanks;

    [Header("Timings")]
    public float displayDuration = 3f;
    public float fadeDuration = 1f;

    public AudioSource backgroundMusic;
    float vol = 0.6f;

    private void Start()
    {
        StartCoroutine(RunCredits());
    }

    void Update()
    {
        backgroundMusic.volume = vol * gameManager.Instance.currentVolume;
    }

    private IEnumerator RunCredits()
    {
        // Ensure overlay starts fully transparent
        overlay.alpha = 0f;

        for (int i = 0; i < creditos.Count; i++)
        {
            // Set background + character
            background.GetComponent<Image>().sprite = creditos[i].backgroundSprite;
            character.GetComponent<Image>().sprite = creditos[i].characterSprite;
            character.GetComponent<Image>().SetNativeSize();
            title.text = creditos[i].title;
            text.text = creditos[i].text;
            thanks.text = creditos[i].thanks;


            // Display slide normally
            yield return new WaitForSeconds(displayDuration);

            // Fade overlay IN
            yield return StartCoroutine(FadeOverlay(1f));

            // Change slide WHILE overlay is on top (if not last slide)
            if (i < creditos.Count - 1)
            {
                background.GetComponent<Image>().sprite = creditos[i + 1].backgroundSprite;
                character.GetComponent<Image>().sprite = creditos[i + 1].characterSprite;
                character.GetComponent<Image>().SetNativeSize();
                title.text = creditos[i+1].title;
                text.text = creditos[i+1].text;
                thanks.text = creditos[i+1].thanks;
            }

            // Fade overlay OUT
            yield return StartCoroutine(FadeOverlay(0f));
        }
        gameManager.Instance.inventory.Clear();
        gameManager.Instance.wasInSR = false;
        gameManager.Instance.wasInSJ = false;
        gameManager.Instance.wasInL = false;
        gameManager.Instance.miniGameName = "";
        gameManager.Instance.currentPlace = "";
        ScreenFader.Instance.TransitionToScene("Main Menu", 0.2f, 0.2f);

    }

    private IEnumerator FadeOverlay(float targetAlpha)
    {
        float startAlpha = overlay.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            overlay.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        overlay.alpha = targetAlpha;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class characterManager : MonoBehaviour
{
    public Dictionary<string, List<Animations>> portraitDict = new Dictionary<string, List<Animations>>();
    public Dictionary<string, Image> characterSlots = new Dictionary<string, Image>();
    private Dictionary<string, Coroutine> runningPortraitCoroutines = new Dictionary<string, Coroutine>();
    private Dictionary<string, Image> characterImages = new Dictionary<string, Image>();


    [Header("UI References")]
    public GameObject character1;
    public GameObject character2;
    public GameObject character3;

    public bool isTyping = false;

    public void LoadSceneCharacters(DialogueScene scene)
    {
        portraitDict.Clear();
        characterImages.Clear();

        Image[] slots = { character1?.GetComponent<Image>(), character2?.GetComponent<Image>(), character3?.GetComponent<Image>() };

        for (int i = 0; i < scene.characters.Count && i < slots.Length; i++)
        {
            DialogueCharacterData character = scene.characters[i];
            string key = character.characterName.ToLower();

            portraitDict[key] = character.animations;
            characterImages[key] = slots[i];

            Image img = slots[i];
            img.sprite = character.defaultSprite;
            img.SetNativeSize();
            img.gameObject.SetActive(true);
        }
    }



    public void InitializeCharacterSlots()
    {
        characterSlots.Clear();

        if (character1 != null)
            characterSlots["slot1"] = character1.GetComponent<Image>();

        if (character2 != null)
            characterSlots["slot2"] = character2.GetComponent<Image>();

        if (character3 != null)
            characterSlots["slot3"] = character3.GetComponent<Image>();
    }

    public void PlayLineAnimation(string speakerName, string expression)
    {
        if (string.IsNullOrEmpty(speakerName)) return;

        string key = speakerName.ToLower();

        if (!portraitDict.ContainsKey(key))
            return;

        if (!characterImages.TryGetValue(key, out Image img))
            return;

        // Find the correct animation by expression
        Animations anim = portraitDict[key].Find(a => a.expression == expression);

        if (anim == null)
        {
            // fallback: default sprite only
            img.sprite = img.sprite; // or set to default
            return;
        }

        StopOtherCharacters(key);
        StartPortraitAnimation(key, img, anim);
    }

    public void StartPortraitAnimation(string characterName, Image img, Animations anim)
    {
        if (runningPortraitCoroutines.ContainsKey(characterName))
        {
            StopCoroutine(runningPortraitCoroutines[characterName]);
            runningPortraitCoroutines.Remove(characterName);
        }

        Coroutine coroutine = StartCoroutine(PlayPortraitAnimation(img, anim));
        StartCoroutine(ScaleCharacter(img, img.transform.localScale, Vector3.one * 1.05f, 0.3f));
        runningPortraitCoroutines[characterName] = coroutine;
    }

    public void StopOtherCharacters(string speakingCharacter)
    {
        List<string> keys = new List<string>(runningPortraitCoroutines.Keys);
        foreach (string key in keys)
        {
            if (key != speakingCharacter)
            {
                StopCoroutine(runningPortraitCoroutines[key]);
                runningPortraitCoroutines.Remove(key);
                if (characterImages.TryGetValue(key, out Image img))
                {
                    StartCoroutine(ScaleCharacter(img, img.transform.localScale, Vector3.one, 0.3f));
                }
            }
        }
    }

    private IEnumerator PlayPortraitAnimation(Image img, Animations anim)
    {
        if (anim.frames == null || anim.frames.Count == 0) yield break;

        int frameCount = anim.frames.Count;
        int currentFrame = 0;
        float frameDuration = 1f / anim.frameRate;

        while (isTyping)
        {
            img.sprite = anim.frames[currentFrame];
            img.SetNativeSize();
            if (img.color.a == 0f) yield return StartCoroutine(FadeImage(img, 0f, 1f, 0.5f));

            currentFrame = (currentFrame + 1) % frameCount;
            yield return new WaitForSeconds(frameDuration);
        }
        img.sprite = anim.frames[0];
    }

    public IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = image.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            image.color = color;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
    }

    public IEnumerator ScaleCharacter(Image img, Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            img.transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            yield return null;
        }

        img.transform.localScale = endScale;
    }

}


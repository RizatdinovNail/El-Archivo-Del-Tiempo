using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class Inputs
{
    public GameObject input;
    public int id;
}



public class Crochet : MonoBehaviour
{
    [Header("Game Settings")]
    public GameObject player;
    public List<Inputs> playerInputs;
    public List<Inputs> showingInputs;
    public List<int> index;
    public List<int> repeatedIndex;
    public List<string> roundText;
    public int currentIndex = 0;
    public int round = 0;
    public int totalRounds = 3;
    public float waitTime;
    public float delayTime;
    public bool playerTurn;
    public GameObject title;

    bool gameIsStarted = false;
    int totalInputs = 3;
    public AudioSource backgroundMusic;


    [Header("Input Animation")]
    public float pressDistance = 15f;
    public float pressDuration = 0.1f;

    [Header("Rules")]
    public GameObject rulesContainer;
    public Button strButton;

    [Header("End")]
    public GameObject endContainer;
    public Button retryButton;
    public Button continueButton;

    private float vol = 0.5f;

    void Start()
    {
        strButton.onClick.AddListener(() =>
        {
            rulesContainer.SetActive(false);
            waitTime = 0.8f;
            delayTime = 0f;
            startGame();
        });

        continueButton.onClick.AddListener(() =>
        {
            gameManager.Instance.wasInL = true;
            gameManager.Instance.miniGameName = "Crochet";
            ScreenFader.Instance.TransitionToScene("Game Scene", 0.3f, 1f);
        });

        retryButton.onClick.AddListener(() =>
        {
            ScreenFader.Instance.TransitionToScene(SceneManager.GetActiveScene().name, 0.3f, 1f);
        });
    }

    void startGame()
    {
        backgroundMusic.Play();
        playerTurn = false;
        if (index.Count != 0) index.Clear();

        for (int i = 0; i < totalInputs; i++)
        {
            index.Add(Random.Range(0, 4));
        }
        if (repeatedIndex.Count != 0) repeatedIndex.Clear();
        gameIsStarted = true;
        StartCoroutine(playInputs());
    }

    void Update()
    {
        if (index.Count == repeatedIndex.Count && gameIsStarted)
        {
            round++;
            if (round < totalRounds)
            {
                currentIndex = 0;
                totalInputs += 1;
                startGame();
            }

            else
            {
                finishGame();
            }
        }

        backgroundMusic.volume = vol * gameManager.Instance.currentVolume;
    }

    IEnumerator playInputs()
    {
        title.GetComponent<TextMeshProUGUI>().text = roundText[round];
        title.SetActive(true);
        yield return new WaitForSeconds(2f);
        title.SetActive(false);
        for (int i = 0; i < index.Count; i++)
        {
            int currentIndex = index[i];
            yield return StartCoroutine(AnimateInputPress(currentIndex));
            yield return new WaitForSeconds(delayTime);

        }

        playerTurn = true;
        currentIndex = 0;
    }

    IEnumerator AnimateInputPress(int idx)
    {
        if (idx < 0 || idx >= showingInputs.Count)
            yield break;

        GameObject go = showingInputs[idx].input;
        if (go == null)
            yield break;

        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null)
            yield break;

        Vector2 originalPos = rt.anchoredPosition;
        Vector2 pressedPos = originalPos - new Vector2(0f, pressDistance);

        // Move down
        float t = 0f;
        while (t < pressDuration)
        {
            rt.anchoredPosition = Vector2.Lerp(originalPos, pressedPos, t / pressDuration);
            t += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = pressedPos;

        // Small hold (optional)
        yield return new WaitForSeconds(waitTime * 0.5f);

        // Move back up
        t = 0f;
        while (t < pressDuration)
        {
            rt.anchoredPosition = Vector2.Lerp(pressedPos, originalPos, t / pressDuration);
            t += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = originalPos;
    }


    void finishGame()
    {
        endContainer.SetActive(true);
    }

    public void restartRound()
    {
        currentIndex = 0;
        startGame();
    }
}

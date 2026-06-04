using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class uiManager : MonoBehaviour
{
    [Header("Text References")]
    public TextMeshProUGUI textOutput;
    public TextMeshProUGUI speakerName;
    public GameObject continueButton;
    [HideInInspector] public int currentCharIndex = 0;
    [HideInInspector] public bool isAnyMenuOpen = false;
    [HideInInspector] public string currentText = "";
    [HideInInspector] public bool phraseEnded = false;

    [Header("Script References")]
    public audioManager AM;
    public gameCore GM;
    public dialogueManager DM;
    public dialogueHistory DH;
    public inventoryInteraction II;
    public characterManager CM;

    [Header("Menu Containers")]
    public GameObject pauseMenuContainer;
    public GameObject dialogueHistoryContainer;
    public GameObject infoWindowContainer;
    public GameObject infoVisuraContainer;
    public TextMeshProUGUI infoWindowText;
    public GameObject inventoryContainer;
    public GameObject mapContainer;
    public GameObject saveLoadContainer;

    [Header("Other")]
    public GameObject door;
    [SerializeField] private float doorFadeDuration = 1f;


    [Header("Choice UI")]
    public GameObject objection;
    public AudioSource objectionSound;
    public GameObject choicePanel;
    public GameObject choiceButtonPrefab;


    [Header("Cinematics")]
    public GameObject locationNameContainer;
    public TextMeshProUGUI locationName;
    public GameObject textContainer;
    private Vector2 locationOriginalPos;
    private Coroutine locationSlideCoroutine;
    private Coroutine locationExitCoroutine;
    private Vector2 textContainerOriginalPos;
    private Coroutine textContainerCoroutine;




    private DialogueChoice currentChoice;

    private float typingSpeed = 0.02f;
    private DialogueLine currentAnim;
    [HideInInspector] public Coroutine typingCoroutine;
    private Coroutine doorFadeCoroutine;

    void Awake()
    {
        locationOriginalPos = locationNameContainer.GetComponent<RectTransform>().anchoredPosition;
        textContainerOriginalPos = textContainer.GetComponent<RectTransform>().anchoredPosition;
    }



    void Update()
    {
        if (DM.currentSceneName != "ayun_returnAfterMiniGame") door.SetActive(false);
        else door.SetActive(true);
        if (GM.clickDoor) door.GetComponent<Button>().interactable = true;
        else door.GetComponent<Button>().interactable = false;
        if (isAnyMenuOpen)
        {
            AM.typingSound.Stop();
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        AM.typingSound.volume = gameManager.Instance.currentVolume;
    }

    public void ResumeTyping()
    {
        typingCoroutine = StartCoroutine(TypeText(currentText));
    }

    public void SetText(DialogueLine currentLine)
    {
        currentAnim = currentLine;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;

            AM.typingSound.Stop();
            textOutput.text = currentLine.text;
            currentCharIndex = 0;
            currentText = "";
            continueButton.SetActive(true);
            phraseEnded = true;

            DH.AddToHistory(currentLine.speaker, textOutput.text);
            showInfoWindowText();

            CM.isTyping = false;
            return;
        }

        phraseEnded = false;
        continueButton.SetActive(false);
        speakerName.text = currentLine.speaker;
        currentText = currentLine.text;

        CM.isTyping = true;
        typingCoroutine = StartCoroutine(TypeText(currentLine.text));
    }

    private IEnumerator TypeText(string originalText)
    {
        textOutput.text = originalText.Substring(0, currentCharIndex);
        AM.typingSound.Play();

        for (int i = currentCharIndex; i < originalText.Length; i++)
        {
            textOutput.text += originalText[i];
            currentCharIndex++;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;
        currentCharIndex = 0;
        phraseEnded = true;
        continueButton.SetActive(true);
        AM.typingSound.Stop();
        showInfoWindowText();
        DH.AddToHistory(speakerName.text, originalText);
        CM.isTyping = false;
    }

    void showInfoWindowText()
    {
        if (GM.mapIsObtained && DM.currentSceneName == "robo_dialogues")
            showInfoWindow("Ahora tienes un mapa. Puedes acceder a él pulsando el icono del papel en la parte superior izquierda de la pantalla.");
        if (GM.showVisura && DM.currentSceneName == "robo_dialogues")
            showVisuraInfo();
        if (GM.showInfoWindow && DM.currentSceneName == "arrabalSanRoque_presenteAfterMiniGame")
        {
            showInfoWindow("Has obtenido la visura del arrabal de San Roque. Puedes viajar al pasado examinándola desde el inventario.");
            II.AddToInventory("Visura San Roque");
        }
        if (GM.showInfoWindow && DM.currentSceneName == "arrabalLoreto_presenteAfterMiniGame")
        {
            showInfoWindow("Has obtenido la visura del arrabal de Loreto. Puedes viajar al pasado examinándola desde el inventario.");
            II.AddToInventory("Visura Loreto");
        }
        if (GM.showInfoWindow && DM.currentSceneName == "arrabalSanJuan_presenteAfterMiniGame")
        {
            showInfoWindow("Has obtenido la visura del arrabal de San Juan. Puedes viajar al pasado examinándola desde el inventario.");
            II.AddToInventory("Visura San Juan");
        }
        if (GM.showDoor)
        {
            doorAppear();
        }
        if (GM.stopMusic)
        {
            DM.backgroundMusic.Stop();
            DM.SFXeffect.Play();
        }
        if(GM.removeCharacter){
            StartCoroutine(CM.FadeImage(CM.character3.GetComponent<Image>(), 1, 0, 0.3f));
        }
    }

    public void openMenuContainer(Button button)
    {
        switch (button.name)
        {
            case "Pause":
                {
                    bool isMiniGame = SceneManager.GetActiveScene().name != "Game Scene";
                    gameManager.Instance.PauseGame(isMiniGame);
                    break;
                }
            case "Dialogue History": dialogueHistoryContainer.SetActive(true); break;
            case "Inventory": II.ToggleContainer(); break;
            case "Load Game": saveLoadContainer.SetActive(true); break;
            case "Save Game": saveLoadContainer.SetActive(true); break;
            case "Settings": break;
            case "Exit":
                {
                    gameManager.Instance.inventory.Clear();
                    gameManager.Instance.wasInSR = false;
                    gameManager.Instance.wasInSJ = false;
                    gameManager.Instance.wasInL = false;
                    gameManager.Instance.miniGameName = "";
                    gameManager.Instance.currentPlace = "";
                    SceneManager.LoadScene("Main Menu");
                    break;
                }
            case "Map": mapContainer.SetActive(true); break;
            default: break;
        }
        CM.isTyping = false;
        isAnyMenuOpen = true;
    }

    public void closeMenuContainer(Button button)
    {
        switch (button.name)
        {
            case "Resume": gameManager.Instance.ResumeGame(); break;
            case "Close DH": dialogueHistoryContainer.SetActive(false); break;
            case "Close Inv":
                {
                    II.closeInventory();
                    break;
                }
            case "Close SL": saveLoadContainer.SetActive(false); break;
            case "Close IW": infoWindowContainer.SetActive(false); break;
            case "Close VI": infoVisuraContainer.SetActive(false); break;
            default: break;
        }

        isAnyMenuOpen = false;
        if (!phraseEnded)
        {
            CM.isTyping = true;
            CM.PlayLineAnimation(currentAnim.key, currentAnim.expression);
            ResumeTyping();
        }
    }

    public void showVisuraInfo()
    {
        infoVisuraContainer.SetActive(true);
        isAnyMenuOpen = true;
    }
    public void showInfoWindow(string infoText)
    {
        infoWindowText.text = infoText;
        infoWindowContainer.SetActive(true);
        isAnyMenuOpen = true;
    }

    public void doorAppear()
    {
        if (door == null) return;

        CanvasGroup cg = door.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            Debug.LogWarning("Door is missing a CanvasGroup component.");
            return;
        }

        if (doorFadeCoroutine != null)
            StopCoroutine(doorFadeCoroutine);

        door.SetActive(true);
        doorFadeCoroutine = StartCoroutine(FadeDoorCanvasGroup(cg));
    }

    private IEnumerator FadeDoorCanvasGroup(CanvasGroup cg)
    {
        cg.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < doorFadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed / doorFadeDuration);
            yield return null;
        }

        cg.alpha = 1f;
    }
    public void clickDoor()
    {
        if (GM.clickDoor)
        {
            ScreenFader.Instance.TransitionToSameScene(() => DM.StartScene("ayunt_inside"), 0.3f, 1f);
        }
    }

    public void ShowChoice(DialogueLine line)
    {
        StartCoroutine(HandleChoicePoint(line));
    }

    private IEnumerator HandleChoicePoint(DialogueLine line)
    {
        isAnyMenuOpen = true;
        CM.isTyping = false;

        if (objection != null && DM.isFirstTime)
        {
            objection.SetActive(true);
            objectionSound?.Play();
            yield return new WaitForSeconds(2f);
            objection.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }

        ShowChoices(line.choice);
    }

    private void ShowChoices(DialogueChoice choice)
    {
        currentChoice = choice;
        choicePanel.SetActive(true);

        foreach (Transform child in choicePanel.transform)
            Destroy(child.gameObject);

        Vector2 pos = Vector2.zero;
        pos.y -= 20f;

        foreach (var option in choice.options)
        {
            var btnObj = Instantiate(choiceButtonPrefab, choicePanel.transform);
            btnObj.transform.localPosition = pos;

            var btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = option.optionText;

            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnOptionSelected(option);
            });

            pos.y += btnObj.GetComponent<RectTransform>().rect.height + 10f;
        }

        continueButton.SetActive(false);
    }

    private void OnOptionSelected(DialogueOption option)
    {
        choicePanel.SetActive(false);
        isAnyMenuOpen = false;

        DM.ResolveChoice(option);
    }

    public IEnumerator PlaySlideAnimation(string locName)
    {
        if (locationSlideCoroutine != null)
            StopCoroutine(locationSlideCoroutine);

        locationSlideCoroutine = StartCoroutine(PlaySlideAnimationRoutine(locName));
        yield return locationSlideCoroutine;
    }

    private IEnumerator PlaySlideAnimationRoutine(string locName)
    {
        locationNameContainer.SetActive(true);

        RectTransform locationRect = locationNameContainer.GetComponent<RectTransform>();
        locationName.text = locName;

        float canvasWidth = locationRect.root.GetComponent<Canvas>()
            .GetComponent<RectTransform>().rect.width;

        Vector2 startPos = new Vector2(-canvasWidth, locationOriginalPos.y);
        Vector2 endPos = locationOriginalPos;

        locationRect.anchoredPosition = startPos;

        float duration = 0.7f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            locationRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        locationRect.anchoredPosition = endPos;
    }

    public IEnumerator PlaySideAnimationEnd()
    {
        if (locationExitCoroutine != null)
            StopCoroutine(locationExitCoroutine);

        locationExitCoroutine = StartCoroutine(PlaySideAnimationEndRoutine());
        yield return locationExitCoroutine;
    }

    private IEnumerator PlaySideAnimationEndRoutine()
    {
        RectTransform locationRect = locationNameContainer.GetComponent<RectTransform>();

        float canvasWidth = locationRect.root
            .GetComponent<Canvas>()
            .GetComponent<RectTransform>().rect.width;

        Vector2 startPos = locationOriginalPos;
        Vector2 endPos = new Vector2(canvasWidth, locationOriginalPos.y);

        locationRect.anchoredPosition = startPos;

        float duration = 0.7f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            locationRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        locationRect.anchoredPosition = locationOriginalPos;
        locationNameContainer.SetActive(false);
    }

    public IEnumerator PlayContainerAnimation()
    {
        if (textContainerCoroutine != null)
            StopCoroutine(textContainerCoroutine);

        textContainerCoroutine = StartCoroutine(PlayContainerAnimationRoutine());
        yield return textContainerCoroutine;
    }

    private IEnumerator PlayContainerAnimationRoutine()
    {
        textContainer.SetActive(true);

        RectTransform textRect = textContainer.GetComponent<RectTransform>();

        float canvasHeight = textRect.root
            .GetComponent<Canvas>()
            .GetComponent<RectTransform>().rect.height;

        Vector2 startPos = new Vector2(textContainerOriginalPos.x, -canvasHeight);
        Vector2 endPos = textContainerOriginalPos;

        textRect.anchoredPosition = startPos;

        float duration = 0.7f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            textRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        textRect.anchoredPosition = endPos;
    }


}

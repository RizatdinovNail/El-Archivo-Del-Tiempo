using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class dialogueManager : MonoBehaviour
{
    public DialogueDatabase database;
    public uiManager UM;
    public gameCore GC;
    public dialogueHistory DH;
    public characterManager CM;
    public GameObject bg;
    public AudioSource backgroundMusic;
    public AudioSource SFXeffect;
    [HideInInspector] public int currentIndex = 0;
    [HideInInspector] public string currentSceneName = "";
    [HideInInspector] public bool dialogueIsFinished = false;
    private DialogueScene currentScene;
    public PlayerControls input;

    public Color opaqueColor;

    [HideInInspector] public bool isChoosingTime = false;
    [HideInInspector] public bool isFirstTime = true;
    private bool isSceneIntroPlaying = false;


    public GameObject people;


    private void Awake()
    {
        input = new PlayerControls();
        people.SetActive(false);
    }

    public void StartScene(string sceneId)
    {
        people.SetActive(false);
        SFXeffect.Stop();   
        dialogueIsFinished = false;
        CM.character1.GetComponent<Image>().color = opaqueColor;
        CM.character2.GetComponent<Image>().color = opaqueColor;
        CM.character3.GetComponent<Image>().color = opaqueColor;
        DH.historyList.Clear();
        currentScene = database.GetScene(sceneId);
        currentSceneName = sceneId;
        CM.InitializeCharacterSlots();
        CM.LoadSceneCharacters(currentScene);
        bg.GetComponent<Image>().sprite = currentScene.background;
        if(currentSceneName == "robo_dialogues") people.SetActive(true);
        if (currentScene == null)
        {
            Debug.LogError($"DialogueScene '{sceneId}' not found in database.");
            return;
        }
        UM.currentCharIndex = 0;
        currentIndex = 0;
        if (currentScene.SFXeffect != null)
        {
            SFXeffect.clip = currentScene.SFXeffect;
        }
        if (currentScene.backgroundMusic != null)
        {
            backgroundMusic.Stop();
            backgroundMusic.clip = currentScene.backgroundMusic;
            backgroundMusic.volume = currentScene.volume * gameManager.Instance.currentVolume;
            backgroundMusic.Play();
        }
        SFXeffect.volume = currentScene.volume * gameManager.Instance.currentVolume;
        StartCoroutine(InitSceneCoroutine());
    }
    private IEnumerator InitSceneCoroutine()
    {
        isSceneIntroPlaying = true;

        switch (currentSceneName)
        {
            case "intro_vistabella":
                {
                    UM.textContainer.SetActive(false);
                    yield return UM.StartCoroutine(UM.PlaySlideAnimation("Vistabella"));
                    yield return new WaitForSeconds(1f);
                    yield return UM.StartCoroutine(UM.PlaySideAnimationEnd());
                    yield return UM.StartCoroutine(UM.PlayContainerAnimation());
                    break;
                }
            case "robo_dialogues":
                {
                    UM.textContainer.SetActive(false);
                    yield return UM.StartCoroutine(UM.PlaySlideAnimation("Ayuntamiento"));
                    yield return new WaitForSeconds(1f);
                    yield return UM.StartCoroutine(UM.PlaySideAnimationEnd());
                    yield return UM.StartCoroutine(UM.PlayContainerAnimation());
                    break;
                }
            case "arrabalLoreto_presente":
                {
                    UM.textContainer.SetActive(false);
                    yield return UM.StartCoroutine(UM.PlaySlideAnimation("Loreto"));
                    yield return new WaitForSeconds(1f);
                    yield return UM.StartCoroutine(UM.PlaySideAnimationEnd());
                    yield return UM.StartCoroutine(UM.PlayContainerAnimation());
                    break;
                }
            case "arrabalLoreto_presente(otro)":
                {
                    UM.textContainer.SetActive(false);
                    yield return UM.StartCoroutine(UM.PlaySlideAnimation("Loreto"));
                    yield return new WaitForSeconds(1f);
                    yield return UM.StartCoroutine(UM.PlaySideAnimationEnd());
                    yield return UM.StartCoroutine(UM.PlayContainerAnimation());
                    break;
                }
            case "arrabalSanJuan_presente":
                {
                    UM.textContainer.SetActive(false);
                    yield return UM.StartCoroutine(UM.PlaySlideAnimation("San Juan"));
                    yield return new WaitForSeconds(1f);
                    yield return UM.StartCoroutine(UM.PlaySideAnimationEnd());
                    yield return UM.StartCoroutine(UM.PlayContainerAnimation());
                    break;
                }
            case "arrabalSanRoque_presente":
                {
                    UM.textContainer.SetActive(false);
                    yield return UM.StartCoroutine(UM.PlaySlideAnimation("San Roque"));
                    yield return new WaitForSeconds(1f);
                    yield return UM.StartCoroutine(UM.PlaySideAnimationEnd());
                    yield return UM.StartCoroutine(UM.PlayContainerAnimation());
                    break;
                }
        }
        isSceneIntroPlaying = false;
        ShowLine();
    }

    private void OnEnable()
    {
        input.DialogueSystem.Enable();
        input.DialogueSystem.Space.performed += NextLine;
    }

    private void OnDisable()
    {
        input.DialogueSystem.Space.performed -= NextLine;
        input.DialogueSystem.Disable();
    }

    public void NextLine(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {

        if (isSceneIntroPlaying)
        {
            return;
        }
        if (!UM.isAnyMenuOpen && !isChoosingTime)
        {
            if (UM.typingCoroutine == null) currentIndex++;
            if (currentIndex < currentScene.dialogues.Count && !dialogueIsFinished)
                ShowLine();
            else
            {
                currentIndex = 0;
                dialogueIsFinished = true;
                switch (currentSceneName)
                {
                    case "intro_vistabella":
                        {
                            ScreenFader.Instance.TransitionToSameScene(() =>
                          {
                              StartScene("robo_dialogues");
                          }, 0.3f, 1f);
                            break;
                        }
                    case "arrabalSanRoque_presente": ScreenFader.Instance.TransitionToScene("cardGame", 0.3f, 1f); break;
                    case "sanRoque_pasado": ScreenFader.Instance.TransitionToScene("StartScene", 0.3f, 1f); break;
                    case "arrabalSanRoque_pasadoAfterMiniGame":
                        {
                            if (gameManager.Instance.wasInL && gameManager.Instance.wasInSJ)
                            {
                                ScreenFader.Instance.TransitionToSameScene(() => StartScene("arrabalSanRoqueAfterPasado(último)"), 0.3f, 1f);
                            }
                            else
                            {
                                ScreenFader.Instance.TransitionToSameScene(() => StartScene("arrabalSanRoqueAfterPasado"), 0.3f, 1f);
                            }
                            break;
                        }
                    case "arrabalLoreto_presente": ScreenFader.Instance.TransitionToScene("SimonSays", 0.3f, 1f); break;
                    case "arrabalLoreto_presente(otro)": ScreenFader.Instance.TransitionToScene("SimonSays", 0.3f, 1f); break;
                    case "arrabalLoreto_pasado": ScreenFader.Instance.TransitionToScene("Crochet", 0.3f, 1f); break;
                    case "arrabalLoreto_pasadoAfterMiniGame":
                        {
                            if (gameManager.Instance.wasInSJ && gameManager.Instance.wasInSR)
                            {
                                ScreenFader.Instance.TransitionToSameScene(() => StartScene("arrabalLoreto_AfterPasado(último)"), 0.3f, 1f);
                            }
                            else
                            {
                                ScreenFader.Instance.TransitionToSameScene(() => StartScene("arrabalLoreto_AfterPasado"), 0.3f, 1f);
                            }
                            break;
                        }
                    case "arrabalSanJuan_presente": ScreenFader.Instance.TransitionToScene("FNF", 0.3f, 1f); break;
                    case "arrabalSanJuan_pasado": ScreenFader.Instance.TransitionToScene("Undertale", 0.3f, 1f); break;
                    case "arrabalSanJuan_pasadoAfterMiniGame":
                        {
                            if (gameManager.Instance.wasInL && gameManager.Instance.wasInSR)
                            {
                                ScreenFader.Instance.TransitionToSameScene(() => StartScene("arrabalSanJuan_AfterPasado(último)"), 0.3f, 1f);
                            }
                            else
                            {
                                ScreenFader.Instance.TransitionToSameScene(() => StartScene("arrabalSanJuan_AfterPasado"), 0.3f, 1f);
                            }
                            break;
                        }
                    case "ayuntamiento_return": ScreenFader.Instance.TransitionToScene("restauracionVisuras", 0.3f, 1f); break;
                    case "ayunt_inside": ScreenFader.Instance.TransitionToSameScene(() => StartScene("juicio"), 0.3f, 1f); break;
                    case "juicio": ScreenFader.Instance.TransitionToSameScene(() => StartScene("final"), 0.3f, 1f); break;
                    case "final": ScreenFader.Instance.TransitionToScene("credits", 0.3f, 1f); break;
                    default: break;
                }
            }
        }

    }

    void ShowLine()
    {
        DialogueLine line = currentScene.dialogues[currentIndex];

        if (line.isChoicePoint)
        {
            isChoosingTime = true;
            line.firstTimeChoice = false;
            UM.ShowChoice(line);
            isFirstTime = true;
            return;
        }

        GC.mapIsObtained = line.mapIsObtained;
        GC.inventoryIsAvaibale = line.inventoryIsUsable;
        GC.showVisura = line.showVisura;
        GC.showInfoWindow = line.showInfoWindow;
        GC.showDoor = line.showDoor;
        GC.clickDoor = line.clickDoor;
        GC.stopMusic = line.stopMusic;
        GC.removeCharacter = line.removeCharacter;

        CM.isTyping = true;
        CM.PlayLineAnimation(line.key, line.expression);
        UM.SetText(line);
        if (line.returnToChoice)
        {
            StartCoroutine(ReturnToChoiceAfterLine(line.returnChoiceIndex));
        }
    }


    public void ResolveChoice(DialogueOption option)
    {
        isChoosingTime = false;

        if (!option.isCorrect)
        {
            currentIndex = option.nextDialogueIndex;
            ShowLine();
            return;
        }

        if (option.nextDialogueIndex >= 0)
            currentIndex = option.nextDialogueIndex;
        else
            currentIndex++;

        ShowLine();
    }

    private IEnumerator ReturnToChoiceAfterLine(int choiceIndex)
    {
        // Wait until typing finishes
        while (UM.typingCoroutine != null)
            yield return null;

        // Wait until player presses continue
        while (!UM.phraseEnded)
            yield return null;

        currentIndex = choiceIndex;
        isFirstTime = false;
        ShowLine();
    }

}

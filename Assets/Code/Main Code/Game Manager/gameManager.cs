using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Inventory
{
    public string name;
    public string description;
    public Sprite image;
}

[System.Serializable]
public class SaveList
{

}

public class gameManager : MonoBehaviour
{
    [Header("Persistent Information")]
    public static gameManager Instance;
    public List<Inventory> inventory;
    public List<SaveList> saveList;
    [Header("Was in the Past Booleans")]
    public bool wasInSR = false;
    public bool wasInSJ = false;
    public bool wasInL = false;
    public PlayerControls input;
    public bool IsPaused { get; private set; }



    [Header("Audio")]
    public bool isAudioMuted = false;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public GameObject miniGamePauseMenu;


    [Header("Scripts")]
    public uiManager UM;
    public audioManager AM;

    [HideInInspector] public string miniGameName;
    public string currentPlace;
    public float currentVolume = 0.5f;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        input = new PlayerControls();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
    }

    private void OnEnable()
    {
        input.DialogueSystem.Enable();
        input.DialogueSystem.Pause.performed += pauseGame;
    }

    private void OnDisable()
    {
        input.DialogueSystem.Pause.performed -= pauseGame;
        input.DialogueSystem.Disable();

    }

    void pauseGame(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (SceneManager.GetActiveScene().name == "Main Menu") return;
        AM.clickSound.Play();

        if (IsPaused)
        {
            return;
        }

        bool isMiniGame = SceneManager.GetActiveScene().name != "Game Scene";
        PauseGame(isMiniGame);
    }

    public void ResumeGame()
    {
        if (!IsPaused) return;

        IsPaused = false;
        Time.timeScale = 1f;

        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (miniGamePauseMenu != null) miniGamePauseMenu.SetActive(false);

        if (UM != null)
            UM.isAnyMenuOpen = false;

        AudioListener.pause = false;
    }

    public void PauseGame(bool isMiniGame)
    {
        if (IsPaused) return;

        IsPaused = true;
        Time.timeScale = 0f;

        if (isMiniGame)
            miniGamePauseMenu.SetActive(true);
        else
            pauseMenu.SetActive(true);

        if (UM != null && !isMiniGame)
            UM.isAnyMenuOpen = true;

        AudioListener.pause = true;
    }

    public void exit()
    {
        AM.clickSound.Play();
        miniGamePauseMenu.SetActive(false);
        SceneManager.LoadScene("Main Menu");
        inventory.Clear();
        wasInL = false;
        wasInSJ = false;
        wasInSR = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Main Code");
        if (obj != null)
        {
            UM = obj.GetComponent<uiManager>();
            pauseMenu = UM.pauseMenuContainer;
        }
        ResumeGame();
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class NoteData
{
    public float time;       // Hit time in seconds
    public int lane;         // Which lane it belongs to
    public string type;      // "tap", "hold", "slide", etc.
    public float length;     // For hold notes (0 = no hold)
    public bool mustHit;     // Whether it's the player's note or the opponent's
}

[System.Serializable]
public class ChartData
{
    public List<NoteData> notes = new List<NoteData>();
}

[System.Serializable]
public class FNFAnimations{
    public List<Sprite> animation;
}

public class BeatGame : MonoBehaviour
{
    [Header("Song Settings")]
    public TextAsset chartFile;
    [HideInInspector] public ChartData chart;
    public AudioSource music;
    [HideInInspector] public bool songFinished = false;
    public List<NoteObject> totalNotes = new List<NoteObject>();
    public NoteObject topNote;

    private double songStartDSPTime;
    private bool isPaused = false;
    private double pausedDSPTime;
    private double pauseOffset;
    public audioManager AM;

    [Header("Score")]
    public GameObject scoreContainer;
    public GameObject continueButton;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI grade;
    public List<string> grades = new List<string>();
    public float animationDuration = 1.5f;
    [HideInInspector] public bool noIsMissed = false;
    [HideInInspector] public int score = 0;

    [Header("Rules")]
    public GameObject rulesContainer;
    public Button startBtn;
    [HideInInspector] public bool gameIsStarted = false;

    [Header("Characters")]
    public GameObject player;
    public GameObject NPC;
    public Sprite defaultPlayer; 
    public Sprite defaultNPC;
    public int animationPlayer;
    public int animationNPC;
    public List<FNFAnimations> npcSprites;
    public List<FNFAnimations> playerSprites;
    public Color colorPlayer;
    public bool NPCstarted = false;
    public bool playerStarted = false;
    public int NPCframe = 0;

    void Awake()
    {
        chart = JsonUtility.FromJson<ChartData>(chartFile.text);
        colorPlayer = player.GetComponent<Image>().color;
    }

    void Start()
    {
        scoreContainer.SetActive(false);
        songFinished = false;
        gameIsStarted = false;
        startBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            rulesContainer.SetActive(false);
            gameIsStarted = true;
            songStartDSPTime = AudioSettings.dspTime + 1.0f;
            music.PlayScheduled(songStartDSPTime);
        });
    }

    public double SongTime =>
        (isPaused ? pausedDSPTime : AudioSettings.dspTime) - songStartDSPTime - pauseOffset;

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Continue()
    {
        gameManager.Instance.miniGameName = "FNF";
        SceneManager.LoadScene("Game Scene");
    }

    void Update()
    {
        if (!songFinished && SongTime >= music.clip.length && gameIsStarted)
        {
            songFinished = true;
            player.GetComponent<Image>().sprite = defaultPlayer;
            player.GetComponent<Image>().SetNativeSize();
            showScore();
        }
        music.volume = gameManager.Instance.currentVolume;
    }

    void showScore()
    {
        if (score < 1000)
        {
            continueButton.SetActive(false);
        }

        scoreContainer.SetActive(true);
        StartCoroutine(scoreAnimation());
    }

    IEnumerator scoreAnimation()
    {
        float elapsed = 0f;
        int startScore = 0;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / animationDuration;
            int animatedScore = Mathf.RoundToInt(Mathf.Lerp(startScore, score, t));

            finalScoreText.text = "Puntuación final: " + animatedScore;

            yield return null;
        }

        finalScoreText.text = "Puntuación final: " + score;

        if (score >= 5513) grade.text = grades[0];
        if (score < 5513 && score >= 4725) grade.text = grades[1];
        if (score < 4725 && score >= 3938) grade.text = grades[2];
        if (score < 3938 && score >= 3150) grade.text = grades[3];
        if (score < 3150 && score >= 2363) grade.text = grades[4];
        if (score < 2363 && score >= 1575) grade.text = grades[5];
        if (score < 1575 && score >= 788) grade.text = grades[6];
        if (score < 788) grade.text = grades[7];
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        pausedDSPTime = AudioSettings.dspTime;

        music.Pause();

        if (AM.backgroundMusic != null) AM.backgroundMusic.Pause();
        if (AM.SFXmusic != null) AM.SFXmusic.Pause();

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        pauseOffset += AudioSettings.dspTime - pausedDSPTime;

        isPaused = false;

        music.UnPause();

        if (AM.backgroundMusic != null) AM.backgroundMusic.UnPause();
        if (AM.SFXmusic != null) AM.SFXmusic.UnPause();

        Time.timeScale = 1f;
    }

}

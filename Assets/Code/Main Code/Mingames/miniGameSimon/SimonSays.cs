using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Nivel actual")]
    public int nivelActual = 1;

    [Header("Botones de ingredientes")]
    public List<Button> botonesIngredientes;

    [Header("Prefabs por nivel")]
    public List<GameObject> nivel1Prefabs;
    public List<GameObject> nivel2Prefabs;
    public List<GameObject> nivel3Prefabs;

    [Header("Panel secuencia")]
    public RectTransform secuenciaPanel;

    [Header("Pantallas finales Nivel 1")]
    public GameObject nivel1_fallos0;
    public GameObject nivel1_fallos1_2;
    public GameObject nivel1_fallos3plus;

    [Header("Pantallas finales Nivel 2")]
    public GameObject nivel2_fallos0;
    public GameObject nivel2_fallos1_2;
    public GameObject nivel2_fallos3plus;

    [Header("Pantallas finales Nivel 3")]
    public GameObject nivel3_fallos0;
    public GameObject nivel3_fallos1_2;
    public GameObject nivel3_fallos3plus;

    [Header("Opciones")]
    public float tiempoVisible = 1f;

    [Header("Rules")]
    public GameObject rulesContainer;
    public Button startButton;

    [Header("Audio")]
    public AudioSource correct;
    public AudioSource incorrect;
    public AudioSource startGame;
    public AudioSource ambientMusic;
    public AudioSource clickSound;

    [Header("Audio settings")]
    public bool isAudioMuted = false;

    private List<GameObject> ingredientesNivelActual;
    private List<int> secuencia = new();
    private List<int> secuenciaCompleta = new();
    private int pasoJugador = 0;
    private int fallos = 0;
    private bool mostrandoSecuencia = false;

    private Dictionary<string, Button> mapaBotones = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            startGame.loop = false;
            startGame.playOnAwake = false;
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        startButton.onClick.AddListener(() =>
        {
            clickSound.Play();
            if (ambientMusic)
            {
                ambientMusic.volume = 0.25f;
                ambientMusic.Play();
            }
            IniciarJuego();
        });
    }

    private void Update()
    {
        float vol = isAudioMuted ? 0f : 1f;

        if (correct) correct.volume = vol * gameManager.Instance.currentVolume;
        if (incorrect) incorrect.volume = vol * 0.4f * gameManager.Instance.currentVolume;
        if (startGame) startGame.volume = vol * 0.5f * gameManager.Instance.currentVolume;
        if (ambientMusic) ambientMusic.volume = vol * 0.5f * gameManager.Instance.currentVolume;
    }

    private void IniciarJuego()
    {
        rulesContainer.SetActive(false);
        SeleccionarIngredientesNivel();
        ApagarPantallasFinales();
        CrearMapaBotones();
        GenerarSecuenciaCompleta();
        AñadirSiguienteIngrediente();
    }

    private void SeleccionarIngredientesNivel()
    {
        ingredientesNivelActual = nivelActual switch
        {
            2 => nivel2Prefabs,
            3 => nivel3Prefabs,
            _ => nivel1Prefabs
        };
    }

    private void CrearMapaBotones()
    {
        mapaBotones.Clear();

        foreach (Button btn in botonesIngredientes)
        {
            btn.onClick.RemoveAllListeners();

            string nombreBoton = btn.name;

            btn.onClick.AddListener(() =>
            {
                if (!mostrandoSecuencia)
                    BotonPulsado(nombreBoton);
            });

            mapaBotones[nombreBoton] = btn;
        }
    }


    private void BotonPulsado(string nombre)
    {
        // STEP 1 — Ingredient not in recipe at all
        if (!ingredientesNivelActual.Any(i => i.name == nombre))
        {
            fallos++;
            incorrect.Play();
            ReiniciarSecuencia();
            return;
        }

        // STEP 2 — Ingredient is in recipe but wrong order
        string esperado = ingredientesNivelActual[secuencia[pasoJugador]].name;

        if (nombre != esperado)
        {
            fallos++;
            incorrect.Play();

            if (fallos >= 3)
            {
                MostrarPantallaFinal();
                return;
            }

            ReiniciarSecuencia();
            return;
        }

        // STEP 3 — Correct input
        pasoJugador++;

        if (pasoJugador >= secuencia.Count)
        {
            if (secuencia.Count < secuenciaCompleta.Count)
                AñadirSiguienteIngrediente();
            else
                MostrarPantallaFinal();
        }
    }


    private void GenerarSecuenciaCompleta()
    {
        secuenciaCompleta.Clear();

        for (int i = 0; i < ingredientesNivelActual.Count; i++)
            secuenciaCompleta.Add(i);

        for (int i = 0; i < secuenciaCompleta.Count; i++)
        {
            int r = Random.Range(i, secuenciaCompleta.Count);
            (secuenciaCompleta[i], secuenciaCompleta[r]) =
                (secuenciaCompleta[r], secuenciaCompleta[i]);
        }

        secuencia.Clear();
    }

    private void AñadirSiguienteIngrediente()
    {
        secuencia.Add(secuenciaCompleta[secuencia.Count]);
        pasoJugador = 0;
        StartCoroutine(MostrarSecuencia());
    }

    private void ReiniciarSecuencia()
    {
        pasoJugador = 0;
        StartCoroutine(MostrarSecuencia());
    }

    private IEnumerator MostrarSecuencia()
    {
        mostrandoSecuencia = true;

        foreach (Transform t in secuenciaPanel)
            Destroy(t.gameObject);

        yield return new WaitForSeconds(0.2f);

        foreach (int index in secuencia)
        {
            GameObject go = Instantiate(ingredientesNivelActual[index], secuenciaPanel);
            RectTransform rt = go.GetComponent<RectTransform>();
            if (rt)
            {
                rt.localScale = Vector3.one;
                rt.anchoredPosition = Vector2.zero;
            }

            yield return new WaitForSeconds(tiempoVisible);
            Destroy(go);
        }

        startGame.Play();
        mostrandoSecuencia = false;
    }

    private void ApagarPantallasFinales()
    {
        nivel1_fallos0?.SetActive(false);
        nivel1_fallos1_2?.SetActive(false);
        nivel1_fallos3plus?.SetActive(false);
        nivel2_fallos0?.SetActive(false);
        nivel2_fallos1_2?.SetActive(false);
        nivel2_fallos3plus?.SetActive(false);
        nivel3_fallos0?.SetActive(false);
        nivel3_fallos1_2?.SetActive(false);
        nivel3_fallos3plus?.SetActive(false);
    }

    private void MostrarPantallaFinal()
    {
        ApagarPantallasFinales();

        GameObject pantalla = nivelActual switch
        {
            1 => fallos == 0 ? nivel1_fallos0 : fallos <= 2 ? nivel1_fallos1_2 : nivel1_fallos3plus,
            2 => fallos == 0 ? nivel2_fallos0 : fallos <= 2 ? nivel2_fallos1_2 : nivel2_fallos3plus,
            _ => fallos == 0 ? nivel3_fallos0 : fallos <= 2 ? nivel3_fallos1_2 : nivel3_fallos3plus
        };

        if (!pantalla) return;

        pantalla.SetActive(true);

        Button[] botones = pantalla.GetComponentsInChildren<Button>(true);

        foreach (Button btn in botones)
        {
            btn.onClick.RemoveAllListeners();

            if (btn.name == "BotonRepetirNivel")
            {
                btn.onClick.AddListener(() =>
                {
                    pantalla.SetActive(false);
                    ReiniciarNivel();
                });
            }
            else if (btn.name == "BotonSiguienteNivel")
            {
                btn.onClick.AddListener(() =>
                {
                    pantalla.SetActive(false);
                    SiguienteNivel();
                });
            }
            else // Continuar
            {
                btn.onClick.AddListener(() =>
                {
                    gameManager.Instance.miniGameName = "Simon Says";
                    SceneManager.LoadScene("Game Scene");
                });
            }
        }
    }

    private void ReiniciarNivel()
    {
        fallos = 0;
        GenerarSecuenciaCompleta();
        AñadirSiguienteIngrediente();
    }

    private void SiguienteNivel()
    {
        nivelActual++;
        if (nivelActual > 3) nivelActual = 1;

        SeleccionarIngredientesNivel();
        fallos = 0;
        CrearMapaBotones();
        GenerarSecuenciaCompleta();
        AñadirSiguienteIngrediente();
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProtaUndertale : MonoBehaviour
{
    private Vector2 moveInput = Vector2.zero;
    private Rigidbody2D rb2D;
    private GameObject go;
    private AudioSource audioSource;
    private bool cubo, agua;
    private int contador, ncubos;
    public float moveSpeed = 5f;
    public Cubo c;
    public Spawner spawner;
    public AudioClip[] audioClips;//La lista de los diferentes sonidos, los pongo en el inspector

    [Header("Rules")]
    public GameObject rulesContainer;
    public Button startGame;
    public bool gameIsStarted = false;
    public AudioSource music;

    bool gameIsFinished = false;


    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        cubo = false;//Indica si el jugador tiene un cubo
        agua = false;//indica si el jugador tiene un cubo con agua
        contador = 0;//Es el contador de victoria
        ncubos = 5;//El número de cubos de la escena, si se cambia el número de cubos hay que cambiar este número
        go = GameObject.FindWithTag("Respawn");
        spawner = go.GetComponent<Spawner>();//Se utiliza más tarde para pasar de nivel
    }

    void Start()
    {
        startGame.GetComponent<Button>().onClick.AddListener(() =>
        {
            gameIsStarted = true;
            rulesContainer.SetActive(false);
            spawner.startGame();
            music.Play();
        });
    }


    void Update()
    {
        if (gameIsStarted)
        {
            Move();//Esto settea hacia dónde se va a mover el jugador

            //Condición de victoria
            if (contador == ncubos)
            {
                if (!gameIsFinished)
                {
                    gameManager.Instance.wasInSJ = true;
                    gameManager.Instance.miniGameName = "Undertale";
                    ScreenFader.Instance.TransitionToScene("Game Scene", 0.3f, 1f);
                }
                gameIsFinished = true;
            }

            music.volume = gameManager.Instance.currentVolume;
            audioSource.volume = gameManager.Instance.currentVolume;
        }
    }

    //Aquí se mueve el jugador
    void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void Move()
    {
        //Esto indica la dirección del jugador
        moveInput = Vector2.zero;

        //Comprobar si se pulsa alguna tecla de movimiento
        if (Keyboard.current != null)
        {
            moveInput.x = (Keyboard.current.dKey.isPressed ? 1 : 0) + (Keyboard.current.aKey.isPressed ? -1 : 0)
                            + (Keyboard.current.rightArrowKey.isPressed ? 1 : 0) + (Keyboard.current.leftArrowKey.isPressed ? -1 : 0);
            moveInput.y = (Keyboard.current.wKey.isPressed ? 1 : 0) + (Keyboard.current.sKey.isPressed ? -1 : 0)
                            + (Keyboard.current.upArrowKey.isPressed ? 1 : 0) + (Keyboard.current.downArrowKey.isPressed ? -1 : 0);
        }

        //Para que las diagonales no hagan que el jugador se mueva más rápido
        if (moveInput.magnitude > 1f) moveInput.Normalize();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Obtener el cubo si no tiene cubos
        if (!cubo && collision.gameObject.CompareTag("Cubo"))
        {
            cubo = true;
            audioSource.clip = audioClips[2];
            audioSource.Play();
            c = collision.GetComponent<Cubo>();
            c.StartMoving();
        }

        //Si tiene un cubo vacío y lo llena
        if (cubo && !agua && collision.gameObject.CompareTag("Agua"))
        {
            agua = true;
            audioSource.clip = audioClips[1];
            audioSource.Play();
            c.CambioAgua();//Esto cambia de cubo vacío a cubo lleno y viceversa
        }

        //Pierde el agua del cubo si se choca con una abeja, la abeja desaparece
        if (agua && collision.gameObject.CompareTag("Abeja"))
        {
            agua = false;
            audioSource.clip = audioClips[0];
            audioSource.Play();
            c.CambioAgua();
            Destroy(collision.gameObject);
        }

        //Deja el cubo de agua al llegar a la zona inicial
        if (agua && collision.gameObject.CompareTag("ZonaCubos"))
        {
            agua = false;
            cubo = false;
            contador++;
            c.Completado();
            spawner.NextLevel();
        }
    }

}

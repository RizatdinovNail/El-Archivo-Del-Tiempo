using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

[Serializable]
public struct CartaData
{
    public int valor;
    public string palo;
}

[Serializable]
public struct JugadaData
{
    public int mayor;
    public int menor;
}


public class cardGame : MonoBehaviour
{
    public CartaUI prefabCartaUI;
    public GameObject cardPrefab;
    public GameObject visuraPrefab;
    public Sprite[] spritesDeCartas;
    public Sprite dorso;
    public CartaData[] baraja = new CartaData[24];
    public JugadaData oros;
    public JugadaData copas;    
    public AudioSource audioSource;
    private GameObject[] cartasOponente = new GameObject[12];
    private CartaUI[] cartasJugador = new CartaUI[12];
    public Transform contenedor;
    public CanvasGroup playerHandCanvasGroup;
    public float animationDuration = 0.5f;
    public JugadaData ultimaOros;
    public JugadaData ultimaCopas;
    public bool isPlayerTurn = true;
    public bool isAnimating = false;
    public Vector3 escalado = new Vector3(1.8f * 90f, 1.6f * 90f, 1.8f * 90f);
    public int playedCardsCount = 0;
    private int NPCplayedCardsCount = 0;
    public bool cartasRepartidas = false;

    [Header("Rules")]
    public GameObject ruleContainer;
    public Button startButton;

    [Header("Audios")]
    public AudioSource shuffle;
    public AudioSource soundtrack;

    void Start()
    {
        shuffle.Stop();
        soundtrack.Stop();
        startButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            ruleContainer.SetActive(false);
            if (gameManager.Instance.isAudioMuted)
            {
                shuffle.volume = 0f;
                soundtrack.volume = 0f;
            }
            shuffle.Play();
            soundtrack.Play();
            StartGame();
        });
    }

    void Update()
    {
        shuffle.volume = gameManager.Instance.currentVolume;
        soundtrack.volume = gameManager.Instance.currentVolume;
        audioSource.volume = gameManager.Instance.currentVolume;
    }

    void StartGame()
    {
        CrearBaraja();
        Mezclar();
        ultimaOros.mayor = ultimaCopas.mayor = ultimaOros.menor = ultimaCopas.menor = -1;

        // Animar jugador
        for (int i = 0; i < spritesDeCartas.Length / 2; i++)
        {
            int posicion = -spritesDeCartas.Length / 4 + i;
            Vector3 posInicial = new Vector3(posicion * 80f, -Math.Abs(posicion) * 150f - 450, 0f);
            //Vector3 startAnimPos = Vector3.zero;
            Vector2 posFinal = new Vector2(posInicial.x, posInicial.y * 0.15f - 200f);
            Quaternion rotFinal = Quaternion.Euler(0, 0, (-posicion) * 4);

            CrearCarta("Carta " + (i + 1), spritesDeCartas[i], posInicial, escalado,
                baraja[i].valor, baraja[i].palo, posicion, i);

            // ANIMAR DESDE CENTRO
            StartCoroutine(AnimarCartaDesdeCentro(
                cartasJugador[i].gameObject,
                posFinal,
                rotFinal,
                escalado,
                delay: i * 0.08f // 0.08s entre cartas
            ));
        }

        // Animar oponente (dorso)
        for (int i = 0; i < spritesDeCartas.Length / 2; i++)
        {
            int posicion = -spritesDeCartas.Length / 4 + i;
            Vector3 posInicial = new Vector3(posicion * 80f, Math.Abs(posicion) * 150f, 0f);
            Vector2 posFinal = new Vector2(posInicial.x, posInicial.y * 0.15f + 275f);
            Quaternion rotFinal = Quaternion.Euler(0, 0, posicion * 4);

            CrearDorso(i, posicion, posInicial, escalado);

            StartCoroutine(AnimarCartaDesdeCentro(
                cartasOponente[i],
                posFinal,
                rotFinal,
                escalado,
                delay: (i + 6) * 0.08f // Empieza después del jugador
            ));
        }

        // Empezar turno después de animaciones
        StartCoroutine(IniciarJuegoDespuesDeAnimacion());
    }

    private IEnumerator IniciarJuegoDespuesDeAnimacion()
    {
        yield return new WaitForSeconds(1.5f); // Tiempo total de animaciones
        cartasRepartidas = true;
        UpdateInteractable();
        ReposicionarManoJugador();
    }

    void CrearBaraja()
    {
        for (int i = 0; i < spritesDeCartas.Length / 2; i++)
        {
            baraja[i] = new CartaData { valor = i + 1, palo = "oros" };
            baraja[i + 12] = new CartaData { valor = i + 1, palo = "copas" };
        }
    }

    void Mezclar()
    {
        for (int i = 0; i < spritesDeCartas.Length; i++)
        {
            CartaData aux;            
            int nuevaPos = UnityEngine.Random.Range(0, spritesDeCartas.Length);
            aux = baraja[nuevaPos];
            baraja[nuevaPos] = baraja[i];
            baraja[i] = aux;

            Sprite sAux = spritesDeCartas[nuevaPos];
            spritesDeCartas[nuevaPos] = spritesDeCartas[i];
            spritesDeCartas[i] = sAux;
        }
    }

    void CrearCarta(string nombre, Sprite sprite, Vector3 posicion, Vector3 scale, int valor, string palo, int p, int i)
    {
        CartaUI nuevaCarta = Instantiate(prefabCartaUI, contenedor);
        nuevaCarta.ConfigurarCarta(nombre, sprite, valor, palo, i, posicion);
        RectTransform rt = nuevaCarta.GetComponent<RectTransform>();
        cartasJugador[i] = nuevaCarta;
        if (rt != null)
        {
            rt.anchoredPosition = new Vector2(0, 0);
            //rt.rotation = Quaternion.Euler(0, 0, (-p) * 4);
            nuevaCarta.transform.localScale = scale;
        }
    }

    void CrearDorso(int i, int p, Vector3 posicion, Vector3 scale)
    {
        GameObject carta = Instantiate(cardPrefab, contenedor);
        RectTransform rt = carta.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, 0);
        //rt.rotation = Quaternion.Euler(0, 0, p * 4);
        carta.transform.localScale = scale;
        cartasOponente[i] = carta;
    }

    public void JugarDorso()
    {
        //if (isPlayerTurn) return;
        UpdateInteractable();

        bool played = false;
        for (int i = 12; i < spritesDeCartas.Length; i++)
        {
            if (baraja[i - 12].valor != -1 && cartasJugador[i - 12] != null) cartasJugador[i - 12].Reposicionar();

            if (!played)
            {
                GameObject carta = cartasOponente[i - 12];
                int posX = ComprobarJugada(i, carta, 1);

                if (posX != -1)
                {
                    NPCplayedCardsCount++;
                    StartCoroutine(MoveCard(carta, posX, i));
                    played = true;
                }
            }
        }

        if (!played)
        {
            EndNPCTurn();
        }
    }

    public int ComprobarJugada(int i, GameObject carta, int valido)
    {
        if (baraja[i].valor == -1) return -1;
        RectTransform rect = carta.GetComponent<RectTransform>();
        int valor = baraja[i].valor;
        string palo = baraja[i].palo;
        if (palo == "oros")
        {
            if (valor != 5)
            {
                if (valor > ultimaOros.mayor + 1 || valor < ultimaOros.menor - 1) return -1;
                if (valido != 0) audioSource.Play();
                if (valor > ultimaOros.mayor)
                {
                    if (valido != 0) ultimaOros.mayor = valor;
                    if (valido != 0) rect.SetAsLastSibling();
                }
                else if (valor < ultimaOros.menor)
                {
                    if (valido != 0) ultimaOros.menor = valor;
                    if (valido != 0) rect.SetAsFirstSibling();
                }
            }
            else
            {
                if (valido != 0) audioSource.Play();
                if (valido != 0) ultimaOros.mayor = ultimaOros.menor = 5;
            }
            return 4;
        }

        if (palo == "copas")
        {
            if (valor != 5)
            {
                if (valor > ultimaCopas.mayor + 1 || valor < ultimaCopas.menor - 1) return -1;
                if (valido != 0) audioSource.Play();
                if (valor > ultimaCopas.mayor)
                {
                    if (valido != 0) ultimaCopas.mayor = valor;
                    if (valido != 0) rect.SetAsLastSibling();
                }
                else if (valor < ultimaCopas.menor)
                {
                    if (valido != 0) ultimaCopas.menor = valor;
                    if (valido != 0) rect.SetAsFirstSibling();
                }
            }
            else
            {
                if (valido != 0) audioSource.Play();
                if (valido != 0) ultimaCopas.mayor = ultimaCopas.menor = 5;
            }
            return -4;
        }
        return -1;
    }

    public IEnumerator MoveCard(GameObject carta, int pos, int index)
    {
        if (index < 12)
        {
            EndPlayerTurn();
        }
        else if (index > 11)
        {
            EndNPCTurn();
        }

        //if (carta == null || isAnimating) yield break;
        isAnimating = true;

        // CORREGIDO: Solo si es carta del jugador (tiene CartaUI)
        CartaUI cartaUI = carta.GetComponent<CartaUI>();
        Image imgComponent = (cartaUI != null) ? cartaUI.ObtenerImagenReal() : carta.GetComponent<Image>();
        if (cartaUI != null)
        {
            cartaUI.jugada = true;
            if (imgComponent != null)
            {
                imgComponent.raycastTarget = false;
                imgComponent.color = cartaUI.originalColor;
            }
        }

        RectTransform rect = carta.GetComponent<RectTransform>();
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = new Vector2((baraja[index].valor - 7) * 50f, pos * 20f + UnityEngine.Random.Range(-0.1f, 0.1f));

        if (imgComponent != null && imgComponent.sprite == dorso)
        {
            imgComponent.sprite = spritesDeCartas[index];
        }

        int inclinacion = UnityEngine.Random.Range(-5, 6);
        Quaternion startRot = rect.rotation;
        Quaternion endRot = Quaternion.Euler(0, 0, inclinacion);

        Vector3 startScale = rect.localScale;
        Vector3 endScale = new Vector3(1.8f * 70f, 1.6f * 70f, 1.8f * 70f);

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            rect.rotation = Quaternion.Lerp(startRot, endRot, t);
            rect.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        rect.anchoredPosition = endPos;
        rect.rotation = endRot;
        rect.localScale = endScale;

        baraja[index].valor = -1;

        ReposicionarManoJugador();

        isAnimating = false;

        if (playedCardsCount == 12) WinGame();
        if (NPCplayedCardsCount == 12) LoseGame();
    }

    private void ReposicionarManoJugador()
    {
        for (int i = 0; i < 12; i++)
        {
            if (cartasJugador[i] != null && baraja[i].valor != -1)
            {
                cartasJugador[i].Reposicionar();
            }
        }
    }

    private void EndPlayerTurn()
    {
        if (playedCardsCount < 12) isPlayerTurn = false;
        //UpdateInteractable();
        StartCoroutine(DelayedNPCTurn());
    }

    private void EndNPCTurn()
    {
        isPlayerTurn = true;
        UpdateInteractable();
    }

    private IEnumerator DelayedNPCTurn()
    {
        yield return new WaitForSeconds(0.5f);
        if (playedCardsCount < 12) JugarDorso();
    }

    private void UpdateInteractable()
    {
        if (playerHandCanvasGroup != null)
        {
            playerHandCanvasGroup.interactable = isPlayerTurn;
            playerHandCanvasGroup.blocksRaycasts = isPlayerTurn;
        }
    }

    public void WinGame()
    {
        gameManager.Instance.miniGameName = "Card Game";
        SceneManager.LoadScene("Game Scene");
    }

    public void LoseGame()
    {
        string nombreEscena = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(nombreEscena);
    }

    private IEnumerator AnimarCartaDesdeCentro(GameObject carta, Vector2 posicionFinal, Quaternion rotacionFinal, Vector3 escalaFinal, float delay = 0f)
    {
        // Esperar delay (para animar una tras otra)
        yield return new WaitForSeconds(delay);

        RectTransform rt = carta.GetComponent<RectTransform>();
        if (rt == null) yield break;

        // POSICIÓN INICIAL: centro de la mesa
        Vector2 centroMesa = new Vector2(0, 0); // Ajusta si tu centro no es (0,0)
        rt.anchoredPosition = centroMesa;

        // Estado inicial
        rt.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-30f, 30f)); // Rotación aleatoria
        rt.localScale = new Vector3(0.3f, 0.3f, 0.3f); // Pequeña al inicio

        // Duración de la animación
        float duration = 0.6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t); // Easing suave

            // Animar posición, rotación y escala
            rt.anchoredPosition = Vector2.Lerp(centroMesa, posicionFinal, t);
            rt.rotation = Quaternion.Lerp(rt.rotation, rotacionFinal, t);
            rt.localScale = Vector3.Lerp(rt.localScale, escalaFinal, t);

            yield return null;
        }

        // Asegurar valores finales
        rt.anchoredPosition = posicionFinal;
        rt.rotation = rotacionFinal;
        rt.localScale = escalaFinal;
    }
}
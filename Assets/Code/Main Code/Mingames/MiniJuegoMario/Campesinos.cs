using UnityEngine;
using UnityEngine.UI;

public class Campesinos : MonoBehaviour
{
    public AudioSource sonidoMulta;

    public bool esMultable = false;  // si este enemigo es multable
    public GameObject mensajeMultarUI; //texto Pulsa M
    public GameObject mensajeExitoUI;
    public GameObject mensajeErrorUI;
    public GameObject nameTagUI;
    public Contador contador;

    private bool jugadorCerca = false;
    private MovimientoJugador jugador;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sonidoMulta = GetComponent<AudioSource>();
        sonidoMulta.volume = sonidoMulta.volume * gameManager.Instance.currentVolume;
        mensajeMultarUI.SetActive(false);
        mensajeExitoUI.SetActive(false);
        mensajeErrorUI.SetActive(false);
        nameTagUI.SetActive(true);

        jugador = GameObject.FindWithTag("Player").GetComponent<MovimientoJugador>();
    }

    // Update is called once per frame
    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.M))
        {
            if (esMultable)
            {
                jugador.Multar();

                mensajeExitoUI.SetActive(true);
                sonidoMulta.Play();
                nameTagUI.SetActive(false);
                StartCoroutine(EsconderMensajeExito());
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;
                contador.Sumar(1);
            }

            else
            {
                // no se puede multar
                mensajeErrorUI.SetActive(true);
                sonidoMulta.Play();
                StartCoroutine(EsconderMensajeError());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
            mensajeMultarUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
            mensajeMultarUI.SetActive(false);
            mensajeErrorUI.SetActive(false);
        }
    }

    private System.Collections.IEnumerator EsconderMensajeExito()
    {
        yield return new WaitForSeconds(1.5f);
        mensajeExitoUI.SetActive(false);
    }

    private System.Collections.IEnumerator EsconderMensajeError()
    {
        yield return new WaitForSeconds(1.5f);
        mensajeErrorUI.SetActive(false);
    }

}

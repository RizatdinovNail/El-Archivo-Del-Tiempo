using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Bandera : MonoBehaviour
{
    public Contador contador;
    public TextMeshProUGUI mensajeBandera;
    public AudioSource sonidoMensaje;

    void Start()
    {
        sonidoMensaje = GetComponent<AudioSource>();
        sonidoMensaje.volume = gameManager.Instance.currentVolume;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (contador.actual >= contador.total)
            {
                SceneManager.LoadScene("WinScene");
            }
            else
            {
                int faltan = contador.total - contador.actual;

                if (faltan == 1)
                {
                    mensajeBandera.text = "¡Vaya! Te falta " + faltan + " multa por poner";//1 sola multa
                }
                else //m�s de 1 multa
                {
                    mensajeBandera.text = "¡Vaya! Te faltan " + faltan + " multas por poner";
                }
                sonidoMensaje.Play();
                mensajeBandera.gameObject.SetActive(true);

                // ocultar despu�s de 2 segundos
                StartCoroutine(EsconderMensaje());
            }
        }
    }

    private System.Collections.IEnumerator EsconderMensaje()
    {
        yield return new WaitForSeconds(2f);
        mensajeBandera.gameObject.SetActive(false);
    }

}


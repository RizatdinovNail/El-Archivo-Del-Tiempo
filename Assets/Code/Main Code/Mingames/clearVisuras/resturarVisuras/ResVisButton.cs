using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class ResVisButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CursorChanger cursor;
    private RestVisManager manejador;
    public GameObject prefab;
    public Transform contenedor;
    private bool primeraVezPulsado = true;
    public Button miBoton;
    public AudioSource changeBrush;
    public Button siguienteBoton;
    [SerializeField] private string tagObjetivo = "ToDestroy";

    void Awake()
    {
        cursor = FindFirstObjectByType<CursorChanger>();
        manejador = FindFirstObjectByType<RestVisManager>();
        changeBrush.volume = gameManager.Instance.currentVolume;
        miBoton.interactable = false;
    }
    public void AlPulsar(int index)
    {
        if (VisuraShader.objetoActual != null)
        {
            VisuraShader.objetoActual.SetLayer((VisuraShader.DirtLayer)index);
            if (index != manejador.GetCursor()) changeBrush.Play();
        }

        cursor.Cambiar(index);
        manejador.ActualizarCursor(index);

        if (primeraVezPulsado)
        {
            GameObject[] objetivos = GameObject.FindGameObjectsWithTag(tagObjetivo);
            foreach (GameObject objetivo in objetivos)
            {
                if (objetivo != null)
                {
                    Destroy(objetivo);
                }
            }
            Instantiate(prefab, contenedor);
            primeraVezPulsado = false;
        }

        if (siguienteBoton != null) siguienteBoton.interactable = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cursor.Reestablecer();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursor.Cambiar(manejador.GetCursor());
    }
}

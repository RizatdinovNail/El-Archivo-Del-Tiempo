using UnityEngine;
using UnityEngine.UI;
using TMPro; // Si usas TextMeshPro
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image darkOverlay;
    [SerializeField] private Image spotlightHole;
    [SerializeField] private TextMeshProUGUI tutorialText; // O Text si no usas TMP

    //[Header("Pasos del Tutorial")]
    [System.Serializable]
    public class TutorialStep
    {
        public GameObject textoExplicacion;
        public GameObject botonObjetivo; // Arrastra el GameObject del botón aquí
    }
    [SerializeField] private List<TutorialStep> pasos = new List<TutorialStep>();

    private int pasoActual = 0;
    private Canvas overlayCanvas;
    private bool tutorialActivo = false;

    void Awake()
    {
        overlayCanvas = GetComponent<Canvas>();
        if (overlayCanvas.worldCamera == null)
        {
            Debug.LogError("¡Asigna la Event Camera al Canvas! (ej. Main Camera)");
        }
        IniciarTutorial(); // Inicialmente oculto
    }

    public void IniciarTutorial()
    {
        if (pasos.Count == 0) 
        {
            Debug.LogError("¡Añade al menos un paso en el Inspector!");
            return;
        }

        pasoActual = 0;
        tutorialActivo = true;
        overlayCanvas.enabled = true;
        SiguientePaso();
    }

    public void FinalizarTutorial()
    {
        tutorialActivo = false;
        overlayCanvas.enabled = false;
        Debug.Log("¡Tutorial completado!");
    }

    private void SiguientePaso()
    {
        if (pasoActual >= pasos.Count)
        {
            FinalizarTutorial();
            return;
        }

        Debug.Log("=== INICIANDO PASO " + pasoActual + " ===");

        var paso = pasos[pasoActual];
        //tutorialText.text = paso.textoExplicacion;

        // Posicionar agujero en el botón objetivo
        if (paso.botonObjetivo != null)
        {
            RectTransform targetRect = paso.botonObjetivo.GetComponent<RectTransform>();
            AjustarAgujero(targetRect);
            
            // AÑADIR LISTENER al botón para avanzar (¡solo este botón funciona!)
            Button btn = paso.botonObjetivo.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(OnBotonPulsado);
            }
        }
        else
        {
            Debug.LogError("¡Botón objetivo es NULL en paso " + pasoActual + "! Arrástralo en Inspector.");
        }

        pasoActual++;
    }

    private void OnBotonPulsado()
    {
        // Remover listener (limpio)
        if (pasoActual > 0)
        {
            Button btn = pasos[pasoActual - 1].botonObjetivo.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveListener(OnBotonPulsado);
            }
        }

        // Pequeña pausa opcional para animación
        Invoke(nameof(SiguientePaso), 0.2f);
    }

    private void AjustarAgujero(RectTransform targetRect)
    {
        if (targetRect == null)
        {
            Debug.LogError("RectTransform del botón es null!");
            return;
        }

        Debug.Log("Ajustando agujero para: " + targetRect.name);

        // CLAVE PARA SCREEN SPACE - CAMERA: Usa la cámara del canvas
        Camera cam = overlayCanvas.worldCamera;
        if (cam == null)
        {
            Debug.LogError("¡Sin cámara asignada! Asigna Event Camera.");
            return;
        }

        // Convierte posición del target a local del canvas overlay
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, targetRect.position);
        RectTransform canvasRect = overlayCanvas.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out Vector2 localPos);

        // Aplica posición y tamaño
        spotlightHole.rectTransform.anchoredPosition = localPos;
        spotlightHole.rectTransform.sizeDelta = targetRect.sizeDelta + new Vector2(60, 60); // Margen para verlo bien

        spotlightHole.gameObject.SetActive(true);

        // Fuerza refresh visual
        Canvas.ForceUpdateCanvases();

        Debug.Log($"Agujero posicionado en {localPos}, tamaño {spotlightHole.rectTransform.sizeDelta}");
    }

    private void OcultarTutorial()
    {
        overlayCanvas.enabled = false;
        spotlightHole.gameObject.SetActive(false);
    }
}
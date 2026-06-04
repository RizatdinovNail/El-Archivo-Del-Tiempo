using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CartaUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Configuraciuon Visual")]
    public RectTransform visualNode;

    public string nombreCarta;
    public Sprite sprite;
    private Image imagen;
    public Color originalColor;  // CORREGIDO: Hecho público para acceso desde Manejador
    private Color hoverColor = new Color(1f, 0.9f, 0.7f, 1f);
    private RectTransform rectPadre;
    public int valor;
    public string palo;
    public bool jugada = false;  // Hecho público si necesitas acceso externo, pero no es necesario
    private cardGame manejador;
    private int posBaraja;
    private Vector2 posicionBase;

    void Awake()
    {
        manejador = FindFirstObjectByType<cardGame>();
        rectPadre = GetComponent<RectTransform>();
        if (visualNode != null)
        {
            imagen = visualNode.GetComponent<Image>();
            if(imagen != null) originalColor = imagen.color;
        }

        else{
            imagen = GetComponent<Image>();
        }

        if(imagen != null) originalColor = imagen.color;
    }

    public void ConfigurarCarta(string nombre, Sprite nuevoSprite, int v, string p, int n, Vector3 posicion)
    {
        nombreCarta = nombre;
        sprite = nuevoSprite;
        valor = v;
        palo = p;
        posBaraja = n;
        if(imagen != null) imagen.sprite = sprite;
        posicionBase = posicion;
        rectPadre.anchoredPosition = new Vector2(posicionBase.x, posicionBase.y * 0.15f - 200f);
    }

    public Image ObtenerImagenReal(){
        if (visualNode != null) return visualNode.GetComponent<Image>();
        return GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(jugada || visualNode == null) return;

        Image img = ObtenerImagenReal();
        if(manejador.ComprobarJugada(posBaraja, gameObject, 0) != -1){
            imagen.color = hoverColor;
        }

        visualNode.anchoredPosition = new Vector2(0, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (jugada || visualNode == null) return;
        imagen.color = originalColor;
        visualNode.anchoredPosition = Vector2.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!manejador.isPlayerTurn || jugada || manejador.isAnimating) return;
        PlayCard(gameObject);
    }

    public void PlayCard(GameObject card)
    {
        int posX = manejador.ComprobarJugada(posBaraja, card, 1);
        if (posX == -1) return;
        manejador.playedCardsCount++;

        visualNode.anchoredPosition = Vector2.zero;
        StartCoroutine(manejador.MoveCard(gameObject, posX, posBaraja));
    }

    public void Reposicionar()
    {
        rectPadre.SetAsLastSibling();
    }
}
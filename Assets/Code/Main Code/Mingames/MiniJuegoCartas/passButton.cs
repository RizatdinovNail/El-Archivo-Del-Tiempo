using UnityEngine;
using UnityEngine.EventSystems;

public class passButton : MonoBehaviour, IPointerClickHandler
{
    private cardGame manejador;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manejador = FindFirstObjectByType<cardGame>();
    }

    // Update is called once per frame
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!manejador.cartasRepartidas) return;
        manejador.isPlayerTurn = false;
        manejador.JugarDorso();
    }
}

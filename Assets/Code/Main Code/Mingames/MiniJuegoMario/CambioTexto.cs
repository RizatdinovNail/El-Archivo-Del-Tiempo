using UnityEngine;

public class CambioTextoTrigger : MonoBehaviour
{
    public CartelUI cartel;
    [TextArea] public string textoIda;
    [TextArea] public string textoVuelta;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            float posJugador = collision.transform.position.x;
            float posTrigger = transform.position.x;

            // Si el jugador está a la izquierda del trigger y se mueve a la derecha
            if (posJugador < posTrigger)
            {
                cartel.CambiarTexto(textoIda);
            }
            else // Si viene por la derecha y entra al trigger
            {
                cartel.CambiarTexto(textoVuelta);
            }
        }
    }
}


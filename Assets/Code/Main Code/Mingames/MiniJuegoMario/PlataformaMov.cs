using UnityEngine;

public class PlataformaMov : MonoBehaviour
{
    public Transform PuntoA;
    public Transform PuntoB;
    public float Velocidad = 2f;

    private Vector3 siguientePosicion;
    private Vector3 ultimaPosicion;

    private GameObject jugadorSobrePlataforma = null;

    void Start()
    {
        siguientePosicion = PuntoB.position;
        ultimaPosicion = transform.position;
    }

    void Update()
    {
        // Mover la plataforma
        transform.position = Vector3.MoveTowards(transform.position, siguientePosicion, Velocidad * Time.deltaTime);

        if (transform.position == siguientePosicion)
        {
            siguientePosicion = (siguientePosicion == PuntoA.position) ? PuntoB.position : PuntoA.position;
        }

        // Calcular desplazamiento de la plataforma
        Vector3 delta = transform.position - ultimaPosicion;

        // Mover al jugador si está sobre la plataforma
        if (jugadorSobrePlataforma != null)
        {
            jugadorSobrePlataforma.transform.position += delta;
        }

        ultimaPosicion = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            jugadorSobrePlataforma = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (jugadorSobrePlataforma == collision.gameObject)
                jugadorSobrePlataforma = null;
        }
    }
}


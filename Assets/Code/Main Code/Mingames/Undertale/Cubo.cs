using UnityEngine;

public class Cubo : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private SpriteRenderer sr;
    private GameObject go;
    private Vector2 direccion,distancia;
    private bool move;
    private float moveSpeed,minDistancia;
    public Sprite cuboVacio;
    public Sprite cuboLleno;
    public ProtaUndertale prota;

    

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = cuboVacio;
        move = false;//indica si el jugador lo ha recogido
        minDistancia = 0.75f;//La distancia que tiene con el jugador
        go = GameObject.FindWithTag("Player");
        prota = go.GetComponent<ProtaUndertale>();
        moveSpeed = prota.moveSpeed;
    }

    void FixedUpdate()
    {
        if (move)
        {
            //Cáculos que hacen que el cubo se mueva hacia el jugador
            distancia = prota.transform.position - transform.position;
            if (distancia.magnitude > minDistancia)
            {
                direccion = distancia.normalized;
                rb2D.MovePosition(rb2D.position + direccion * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    //Se cambia el sprite del cubo
    public void CambioAgua()
    {
        if (sr.sprite == cuboLleno)
        {
            sr.sprite = cuboVacio;
        }
        else
        {
            sr.sprite = cuboLleno;
        }

    }

    public void StartMoving()
    {
        move = true;
    }

    //Elimina todo lo que hace que el cubo se mueva para que se quede el sprite en el suelo
    public void Completado()
    {
        move = false;
        Destroy(rb2D);
        Destroy(GetComponent<Collider2D>());
    }
}

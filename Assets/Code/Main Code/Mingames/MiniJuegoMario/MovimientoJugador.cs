using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    public AudioSource sonidoSalto;
    public Rigidbody2D rb;
    public Animator animator;
    bool miraDerecha = true;

    [Header("Movimiento")]
    public float movVelocidad = 4f;
    public float movHorizontal;

    [Header("Salto")]
    public float impulsoSalto = 10f;
    public float multiplicadorSaltoCorto = 0.5f;
    public int maxSaltos = 2;
    private int saltosRestantes = 2;

    [Header("Comprobar Suelo")]
    public Transform sueloCompPos;
    public Vector2 sueloCompTamaño = new Vector2(0.7f, 0.2f);
    public LayerMask capaSuelo;

    [Header("Gravedad")]
    public float gravedadBase = 2f;
    public float maxVelCaida = 18f;
    public float caidaGravedadMult = 2f;


    void Start()
    {
        sonidoSalto = GetComponent<AudioSource>();
        sonidoSalto.volume = sonidoSalto.volume * gameManager.Instance.currentVolume;
    }

    // Update is called once per frame
    void Update()
    {
        comprobarSuelo();
        Gravedad();
        Voltear();

        //Movimiento
        rb.linearVelocity = new Vector2(movHorizontal * movVelocidad, rb.linearVelocity.y);

        //Animaciones
        animator.SetFloat("magnitud", rb.linearVelocity.magnitude);
        animator.SetFloat("velocidadY", rb.linearVelocity.y);
    }

    public void Mover(InputAction.CallbackContext context)
    {
        movHorizontal = context.ReadValue<Vector2>().x;
    }

    public void Saltar(InputAction.CallbackContext context)
    {
        if (saltosRestantes > 0)
        {
            if (context.performed) //sostiene salto
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, impulsoSalto);
                animator.SetTrigger("salto");
                sonidoSalto.Play();
                saltosRestantes--;
            }
            else if (context.canceled && rb.linearVelocity.y > 0) //salto peque�o
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * multiplicadorSaltoCorto);
                animator.SetTrigger("salto");
                saltosRestantes--;
            }
        }
    }

    public void Multar()
    {
        if (animator != null)
        {
            animator.SetTrigger("multar");
        }
    }

    public void Gravedad()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = gravedadBase * caidaGravedadMult;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxVelCaida));
        }
        else
        {
            rb.gravityScale = gravedadBase;
        }
    }

    private void Voltear()
    {
        if (miraDerecha && movHorizontal < 0 || !miraDerecha && movHorizontal > 0)
        {
            miraDerecha = !miraDerecha;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void comprobarSuelo()
    {
        if (Physics2D.OverlapBox(sueloCompPos.position, sueloCompTamaño, 0, capaSuelo))
        {
            saltosRestantes = maxSaltos;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(sueloCompPos.position, sueloCompTamaño);
    }
}

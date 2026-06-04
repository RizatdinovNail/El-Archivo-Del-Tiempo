using UnityEngine;

public class Abejas : MonoBehaviour
{
    private Rigidbody2D rb2D;
    public Vector2 moveDir = Vector2.zero;//Esto se cambia al spawnear la abeja en spawner e indica la dirección
    public float moveSpeed = 0f;//Esta velocidad la cambio manualmente al iniciar las abejas en el spawner

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        //Esto es para que no se muevan más rápidas al ir en diagonal
        if (moveDir.magnitude > 1f) moveDir.Normalize();
    }

    void Update()
    {
        BoundsDelete();
    }

    void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

    //Aquí elimino a las abejas cuándo se alejan mucho (para que no se salgan del pozo)
    void BoundsDelete()
    {
        //El primer if está para dejar el cuadrado de arriba a la izquierda como una zona segura
        if (transform.position.x < -1.7 && transform.position.y > 1.7)
        {
            Destroy(gameObject);
        }
        else if (transform.position.x > 5 || transform.position.x < -5 || transform.position.y > 5 || transform.position.y < -3)
        {
            Destroy(gameObject);
        }
    }
}

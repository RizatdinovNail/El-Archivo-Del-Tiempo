using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{

    public float velocidad = 0.5f;  // entre 0 y 1 normalmente
    private Transform camara;
    private Vector3 posicionAnteriorCamara;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camara = Camera.main.transform;
        posicionAnteriorCamara = camara.position;
    }

    void LateUpdate()
    {
        Vector3 movimientoCamara = camara.position - posicionAnteriorCamara;

        // Mover esta capa en proporción al movimiento de la cámara
        transform.position += new Vector3(movimientoCamara.x * velocidad, 0, 0);

        posicionAnteriorCamara = camara.position;
    }
}

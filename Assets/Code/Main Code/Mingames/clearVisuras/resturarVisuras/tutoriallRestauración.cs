using UnityEngine;
using UnityEngine.UI;


public class TutorialRestauracion : MonoBehaviour
{
    public Button miBoton;
    public GameObject prefab;
    public Transform contenedor; 
    public void Cerrar()
    {
        GameObject siguienteTexto = Instantiate(prefab, contenedor);
        Destroy(transform.parent.gameObject);

        miBoton.interactable = true;
    }
}

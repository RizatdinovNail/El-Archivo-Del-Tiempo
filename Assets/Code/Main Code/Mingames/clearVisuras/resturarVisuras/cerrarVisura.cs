using UnityEngine;

public class cerrarVisura : MonoBehaviour
{
    public void Cerrar()
    {
        Destroy(transform.parent.gameObject);
    }
}

using UnityEngine;

public class AlwaysBackground : MonoBehaviour
{
    private Transform parent;
    private int initialSiblingIndex;

    void Start()
    {
        parent = transform.parent;
        initialSiblingIndex = transform.GetSiblingIndex(); // Guarda su posición original
    }

    void LateUpdate()
    {
        // SIEMPRE lo mandamos al fondo (índice 0)
        if (transform.GetSiblingIndex() != 0)
        {
            transform.SetAsFirstSibling();
        }
    }
}
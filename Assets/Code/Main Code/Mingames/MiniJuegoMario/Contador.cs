using UnityEngine;
using TMPro;

public class Contador : MonoBehaviour
{
    public TextMeshProUGUI textoContador;

    [Header("Progreso")]
    public int actual = 0;
    public int total = 3;

    void Start()
    {
        ActualizarTexto();
    }

    public void Sumar(int cantidad)
    {
        actual += cantidad;

        // evitar pasarse del total
        if (actual > total)
            actual = total;

        ActualizarTexto();
    }

    void ActualizarTexto()
    {
        textoContador.text = actual + "/" + total;
    }
}
using UnityEngine;
using TMPro;

public class CartelUI : MonoBehaviour
{
    public TextMeshProUGUI texto;

    public void CambiarTexto(string nuevoTexto)
    {
        texto.text = nuevoTexto;
    }
}


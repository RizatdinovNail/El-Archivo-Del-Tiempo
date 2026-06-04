using UnityEngine;

public class RestVisManager : MonoBehaviour
{
    public int visurasRestauradas = 0;
    private int cursorType = -1;

    public int Actualizar()
    {
        return visurasRestauradas++;
    }

    public void ActualizarCursor(int i)
    {
        cursorType = i;
    }
    public bool allVisurasAreClean()
    {
        return visurasRestauradas >= 3;
    }

    public int GetCursor()
    {
        return cursorType;
    }
}
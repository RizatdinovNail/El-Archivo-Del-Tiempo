using UnityEngine;

public class CursorChanger : MonoBehaviour
{
    public Texture2D[] cursorTexture;
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        //Cambiar(0);
    }

    public void Reestablecer()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void Cambiar(int i)
    {
    if (i < 0 || i >= cursorTexture.Length) return;

    Texture2D tex = cursorTexture[i];
    Vector2 centerHotspot = new Vector2(0, tex.height);

    Cursor.SetCursor(tex, centerHotspot, CursorMode.ForceSoftware);
    }

}

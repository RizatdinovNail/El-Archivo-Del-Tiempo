using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class VisuraShader : MonoBehaviour
{
    private Camera cam;

    public Texture2D dirtMaskBase;      // Ahora debe ser RGB (puedes tener R=G=B=1 al inicio)
    public Texture2D brush;             // Tu pincel actual (en escala de grises o con alpha)
    public Texture2D brush2;
    public Texture2D brush3;

    public Material material;
    public GameObject VisuraPrefab;
    private float duration = 1f;

    private Texture2D templateDirtMask;
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;
    private RestVisManager manejador;
    private Material materialInstanciado;
    public AudioSource pintando;
    public AudioClip brushSound1;
    public AudioClip brushSound2;

    public static VisuraShader objetoActual;
    private CursorChanger cursor;
    //private int cursorN;
    private static readonly int DirtMaskID = Shader.PropertyToID("_DirtMask");

    // ====== NUEVO: SISTEMA DE 3 CAPAS ======
    public enum DirtLayer { Layer1, Layer2, Layer3 }
    [Header("Configuración de Capas")]
    public DirtLayer currentLayer = DirtLayer.Layer1;

    [Tooltip("Bloquear capas hasta limpiar la anterior")]
    private bool secuencial = false;

    private bool layer1Clean = false;
    private bool layer2Clean = false;
    private bool layer3Clean = false;

    public AudioSource backgroundMusic;
    float vol = 0.6f;


    // ================== ADDITIONAL PARTS ==============
    private static bool winTriggered = false;
    int timesWinIsCalled = 1;


    // Colores para feedback visual del pincel activo
    private readonly Color[] layerColors = new Color[]
    {
        new Color(1, 0.3f, 0.3f, 0.7f), // Rojo suave → Capa 1
        new Color(0.3f, 1, 0.3f, 0.7f), // Verde suave → Capa 2
        new Color(0.3f, 0.7f, 1, 0.7f)  // Azul suave → Capa 3
    };

    private void Awake()
    {
        winTriggered = false;
        backgroundMusic.loop = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        materialInstanciado = new Material(material);
        spriteRenderer.material = materialInstanciado;

        cursor = FindFirstObjectByType<CursorChanger>();
    }

    private void Start()
    {
        CreateTexture();
        backgroundMusic.Play();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null) Debug.LogError("¡Añade un Collider2D!");

        cam = Camera.main;
        manejador = FindFirstObjectByType<RestVisManager>();

        int cursorActual = manejador.GetCursor();
        SetLayer((DirtLayer)cursorActual);
        //cursor.Cambiar(cursorActual);

        transform.position = new Vector3(-20, transform.position.y, transform.position.z);
        StartCoroutine(AnimarVisura(transform.position, Vector3.zero, transform.localScale, transform.localScale, false));
    }

    private void OnEnable() => objetoActual = this;
    private void OnDisable() { if (objetoActual == this) objetoActual = null; }

    private void Update()
    {
        // Cambio de capa con teclas 1-2-3
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetLayer(DirtLayer.Layer1);
            cursor.Cambiar(0);
            manejador.ActualizarCursor(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetLayer(DirtLayer.Layer2);
            cursor.Cambiar(1);
            manejador.ActualizarCursor(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetLayer(DirtLayer.Layer3);
            cursor.Cambiar(2);
            manejador.ActualizarCursor(2);
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        bool buttonDown = Input.GetMouseButton(0);
        bool isOverCollider = (myCollider != null && myCollider.OverlapPoint(mousePos2D));
        bool isCleaning = buttonDown && isOverCollider;

        if (isCleaning)
        {
            if (!pintando.isPlaying)
            {
                pintando.Play();
            }
        }

        pintando.volume = gameManager.Instance.currentVolume;
        backgroundMusic.volume = vol * gameManager.Instance.currentVolume;


        if (myCollider != null && myCollider.OverlapPoint(mousePos2D))
        {
            Vector2 localPos = mouseWorldPos - transform.position;
            Vector2 uv = new Vector2(
                (localPos.x / spriteRenderer.bounds.size.x) + 0.5f,
                (localPos.y / spriteRenderer.bounds.size.y) + 0.5f
            );

            PaintAtUV(uv);
        }
    }

    public void SetLayer(DirtLayer layer)
    {
        if (secuencial)
        {
            if (layer == DirtLayer.Layer2 && !layer1Clean) return;
            if (layer == DirtLayer.Layer3 && !layer2Clean) return;
        }

        currentLayer = layer;
        //Debug.Log($"Capa activa: {layer + 1}");
        // Feedback visual opcional: cambiar color del cursor o un indicador UI
    }

    void PaintAtUV(Vector2 uv)
    {
        int px = Mathf.RoundToInt(uv.x * templateDirtMask.width);
        int py = Mathf.RoundToInt(uv.y * templateDirtMask.height);

        int left = Mathf.Max(0, px - brush.width / 2);
        int right = Mathf.Min(templateDirtMask.width, px + brush.width / 2);
        int bottom = Mathf.Max(0, py - brush.height / 2);
        int top = Mathf.Min(templateDirtMask.height, py + brush.height / 2);

        // Usamos Color32 para máxima velocidad
        Color32[] pixels = templateDirtMask.GetPixels32();
        Color32[] brushPixels = brush.GetPixels32();
        Color32[] brushPixels2 = brush2.GetPixels32();
        Color32[] brushPixels3 = brush3.GetPixels32();

        int width = templateDirtMask.width;

        for (int y = bottom; y < top; y++)
        {
            for (int x = left; x < right; x++)
            {
                int brushX = x - (px - brush.width / 2);
                int brushY = y - (py - brush.height / 2);
                if (brushX < 0 || brushY < 0 || brushX >= brush.width || brushY >= brush.height) continue;

                Color32 brushPixel = brushPixels[brushY * brush.width + brushX];
                Color32 brushPixel2 = brushPixels2[brushY * brush.width + brushX];
                Color32 brushPixel3 = brushPixels3[brushY * brush.width + brushX];

                if (brushPixel.a == 0) continue;

                int index = y * width + x;
                Color32 maskPixel = pixels[index];
                float strength;


                switch (currentLayer)
                {
                    case DirtLayer.Layer1:
                        strength = 1 - (brushPixel.g / 255f);
                        maskPixel.g = (byte)Mathf.Lerp(maskPixel.g, 0, strength * Time.deltaTime * 30f);
                        break;
                    case DirtLayer.Layer2:
                        strength = 1 - (brushPixel2.g / 255f);
                        maskPixel.b = (byte)Mathf.Lerp(maskPixel.b, 0, strength * Time.deltaTime * 30f);
                        break;
                    case DirtLayer.Layer3:
                        strength = 1 - (brushPixel3.g / 255f);
                        maskPixel.r = (byte)Mathf.Lerp(maskPixel.r, 0, strength * Time.deltaTime * 30f);
                        break;
                }

                pixels[index] = maskPixel;
            }
        }

        templateDirtMask.SetPixels32(pixels);
        templateDirtMask.Apply();

        // Comprobar progreso de capas cada cierto tiempo (optimizado)
        if (Time.frameCount % 50 == 0) CheckLayersProgress();
    }

    void CheckLayersProgress()
    {
        Color32[] pixels = templateDirtMask.GetPixels32();

        layer1Clean = true; layer2Clean = true; layer3Clean = true;

        foreach (Color32 p in pixels)
        {
            if (p.r == 255) layer1Clean = false;
            if (p.g == 255) layer2Clean = false;
            if (p.b == 255) layer3Clean = false;
        }

        if (layer1Clean && layer2Clean && layer3Clean)
            Win();
    }

    /*public static void PulsarBotonLimpiarTodo()
    {
        if (objetoActual != null)
            objetoActual.CheckIfClean();
    }

    private void CheckIfClean()
    {
        if (layer1Clean && layer2Clean && layer3Clean)
        {
            //Debug.Log("¡TODO LIMPIO! Victoria");
        }
        else
        {
            //Debug.Log("Aún quedan suciedades por limpiar");
        }
    }*/

    private void CreateTexture()
    {
        templateDirtMask = new Texture2D(dirtMaskBase.width, dirtMaskBase.height, TextureFormat.RGBA32, false);
        templateDirtMask.SetPixels32(dirtMaskBase.GetPixels32());
        templateDirtMask.Apply();

        materialInstanciado.SetTexture(DirtMaskID, templateDirtMask);
    }

    private void Win()
    {
        //manejador.ActualizarCursor(cursorN);
        if (winTriggered) return;
        winTriggered = true;
        timesWinIsCalled++;

        bool crearNuevo = true;
        manejador.Actualizar();
        Debug.Log(manejador.visurasRestauradas);
        if (manejador != null && manejador.allVisurasAreClean())
        {
            crearNuevo = false;
            Debug.Log("End");
            gameManager.Instance.miniGameName = "ClearVisuras";
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            ScreenFader.Instance.TransitionToScene("Game Scene", 0.3f, 1f);
            return;
        }

        StartCoroutine(AnimarVisura(transform.position, new Vector3(20, transform.position.y, transform.position.z),
            transform.localScale, transform.localScale * 0.8f, crearNuevo));
    }

    private IEnumerator AnimarVisura(Vector3 startPos, Vector3 endPos, Vector3 startScale, Vector3 endScale, bool crearNuevo)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        if (endPos.x > 10)
        {
            if (crearNuevo)
            {
                Destroy(gameObject);
                Instantiate(VisuraPrefab, transform.parent);
                yield return null;
            }
        }
    }

    // Feedback visual opcional del pincel activo
    /* private void OnGUI()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mouse = Input.mousePosition;
            mouse.y = Screen.height - mouse.y;
            GUI.color = layerColors[(int)currentLayer];
            GUI.Label(new Rect(mouse.x + 15, mouse.y - 10, 200, 30), $"Capa {(int)currentLayer + 1}");
        }
    } */
}
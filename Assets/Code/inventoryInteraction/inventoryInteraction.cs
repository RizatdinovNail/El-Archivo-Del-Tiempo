using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class inventoryInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image imagen;
    private Color originalColor;
    private Color hoverColor;

    [Header("GameCore Reference")]
    public gameCore gameCore;

    [Header("UI References")]
    public GameObject inventoryContainer;
    public GameObject objParent;
    public GameObject objectPrefab;
    Vector2 objPrefabPosition;

    [Header("Visuras Description")]
    public TextAsset visuraSanRoqueText;
    public TextAsset visuraSanJuanText;
    public TextAsset visuraLoretoText;

    [Header("Visuras sprite")]
    public Sprite visuraSanRoque;
    public Sprite visuraSanJuan;
    public Sprite visuraLoreto;

    void Start()
    {
        imagen = GetComponent<Image>();
        originalColor = imagen.color;
        hoverColor = new Color(1f, 0.9f, 0.7f, 1f);
        imagen.color = originalColor;
        objPrefabPosition = objectPrefab.transform.localPosition;
    }


    void Update()
    {
        if (gameCore.inventoryIsAvaibale)
        {
            gameObject.GetComponent<Button>().interactable = true;
        }

        else
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        imagen.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imagen.color = originalColor;
    }

    public void ToggleContainer()
    {
        if (!inventoryContainer.activeSelf)
        {
            showInventory();
        }
    }

    public void AddToInventory(string visuraType)
    {
        if (gameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found.");
            return;
        }

        if (gameManager.Instance.inventory.Exists(v => v.name == visuraType))
            return;

        TextAsset textAsset = null;
        Sprite sprite = null;

        switch (visuraType)
        {
            case "Visura San Roque":
                textAsset = visuraSanRoqueText;
                sprite = visuraSanRoque;
                break;

            case "Visura San Juan":
                textAsset = visuraSanJuanText;
                sprite = visuraSanJuan;
                break;

            case "Visura Loreto":
                textAsset = visuraLoretoText;
                sprite = visuraLoreto;
                break;

            default:
                Debug.LogWarning($"Unknown visura type: {visuraType}");
                return;
        }

        if (textAsset == null)
        {
            Debug.LogError($"Missing TextAsset for {visuraType}");
            return;
        }

        VisuraData data = JsonUtility.FromJson<VisuraData>(textAsset.text);

        gameManager.Instance.inventory.Add(new Inventory
        {
            name = visuraType,
            description = data.description,
            image = sprite
        });
    }

    void showInventory()
    {
        foreach (Inventory obj in gameManager.Instance.inventory)
        {
            Inventory localObj = obj;

            GameObject newObj = Instantiate(objectPrefab);

            newObj.SetActive(true);
            newObj.transform.SetParent(objParent.transform, false);
            newObj.name = localObj.name;
            newObj.GetComponent<Image>().sprite = localObj.image;

            newObj.transform.localPosition = objPrefabPosition;

            var click = newObj.GetComponent<clickObject>();

            newObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                click.ShowObject(localObj.image, localObj.description, localObj.name);
            });

            objPrefabPosition.x += objectPrefab.GetComponent<RectTransform>().rect.width + 10f;

        }

        inventoryContainer.SetActive(true);
    }

    public void closeInventory()
    {
        foreach (Transform child in objParent.transform)
        {
            if (child.name != "Prefab")
            {
                Destroy(child.gameObject);
            }
        }

        objPrefabPosition = objectPrefab.transform.localPosition;
        inventoryContainer.SetActive(false);
    }

}

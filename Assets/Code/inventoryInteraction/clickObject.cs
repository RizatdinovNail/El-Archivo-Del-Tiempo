using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class clickObject : MonoBehaviour
{
    [Header("UI References")]
    public GameObject objContainer;
    public Image objImage;
    public TextMeshProUGUI objDescription;
    public Button closeObjContainerButton;
    public Button goToPastButton;

    [Header("Script Ref")]
    public dialogueManager DM;
    public uiManager UM;

    string currentOpenVisura = "";

    public void ShowObject(Sprite image, string description, string name)
    {
        objImage.sprite = image;
        objDescription.text = description;
        currentOpenVisura = name;
        goToPastButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            GoToPast(currentOpenVisura);
        });
        objContainer.SetActive(true);
    }

    public void closeDescription()
    {
        objContainer.SetActive(false);
    }

    void GoToPast(string visuraName)
    {
        switch (visuraName)
        {
            case "Visura San Roque":
                {
                    if (gameManager.Instance.wasInSR) { return; }
                    UM.inventoryContainer.SetActive(false);
                    objContainer.SetActive(false);
                    UM.phraseEnded = true;
                    UM.isAnyMenuOpen = false;
                    DM.StartScene("sanRoque_pasado");
                    break;
                }
            case "Visura San Juan":
                {
                    if (gameManager.Instance.wasInSJ) { return; }
                    UM.inventoryContainer.SetActive(false);
                    objContainer.SetActive(false);
                    UM.phraseEnded = true;
                    UM.isAnyMenuOpen = false;
                    DM.StartScene("arrabalSanJuan_pasado");
                    break;
                }
            case "Visura Loreto":
                {
                    if (gameManager.Instance.wasInL) { return; }
                    UM.inventoryContainer.SetActive(false);
                    objContainer.SetActive(false);
                    UM.phraseEnded = true;
                    UM.isAnyMenuOpen = false;
                    DM.StartScene("arrabalLoreto_pasado");
                    break;
                }
            default: break;
        }
    }
}

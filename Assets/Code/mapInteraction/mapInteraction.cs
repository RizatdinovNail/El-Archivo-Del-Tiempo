using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Iconos
{
    public string place;
    public GameObject icono;
}
public class mapInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image imagen;
    private Color originalColor;
    private Color hoverColor;
    public List<Iconos> icons;

    [Header("GameCore Reference")]
    public gameCore gameCore;

    [Header("UI References")]
    public GameObject bigMapContainer;


    [Header("Script References")]
    public dialogueManager DM;
    public uiManager UM;


    void Start()
    {
        imagen = GetComponent<Image>();
        originalColor = imagen.color;
        hoverColor = new Color(1f, 0.9f, 0.7f, 1f);
        imagen.color = originalColor;
    }


    void Update()
    {
        if (gameCore.mapIsObtained)
        {
            gameObject.GetComponent<Button>().interactable = true;
        }

        else
        {
            gameObject.GetComponent<Button>().interactable = false;
        }

        if (bigMapContainer.activeSelf)
        {
            changeIcon();
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

    public void ToggleMap()
    {
        bigMapContainer.SetActive(!bigMapContainer.activeSelf);
    }

    public void ChangeScene(Button button)
    {
        switch (button.name)
        {
            case "San Roque":
                {
                    DM.currentIndex = 0;
                    if (!gameManager.Instance.wasInSR)
                    {
                        ScreenFader.Instance.TransitionToSameScene(() =>
                          {
                              DM.StartScene("arrabalSanRoque_presente");
                          }, 0.3f, 1f);
                    }
                    else { return; }
                    break;
                }
            case "San Juan":
                {
                    DM.currentIndex = 0;
                    if (!gameManager.Instance.wasInSJ)
                    {
                        ScreenFader.Instance.TransitionToSameScene(() =>
                          {
                              DM.StartScene("arrabalSanJuan_presente");
                          }, 0.3f, 1f);
                    }
                    else return;
                    break;
                }
            case "Loreto":
                {
                    DM.currentIndex = 0;
                    if (!gameManager.Instance.wasInL)
                    {

                        if (!gameManager.Instance.wasInSJ)
                        {
                            ScreenFader.Instance.TransitionToSameScene(() =>
                              {
                                  DM.StartScene("arrabalLoreto_presente(otro)");
                              }, 0.3f, 1f);
                        }
                        else
                        {
                            ScreenFader.Instance.TransitionToSameScene(() =>
                              {
                                  DM.StartScene("arrabalLoreto_presente");
                              }, 0.3f, 1f);
                        }
                    }
                    else
                    {
                        return;
                    }
                    break;
                }
            case "Ayuntamiento":
                {
                    DM.currentIndex = 0;
                    if (gameManager.Instance.wasInL && gameManager.Instance.wasInSJ && gameManager.Instance.wasInSR)
                    {
                        ScreenFader.Instance.TransitionToSameScene(() =>
                          {
                              DM.StartScene("ayuntamiento_return");
                          }, 0.3f, 1f);
                    }
                    else return;
                    break;
                }
            default: break;
        }
        gameManager.Instance.currentPlace = button.name;
        bigMapContainer.SetActive(false);
        UM.isAnyMenuOpen = false;
    }

    void changeIcon()
    {
        foreach (Iconos icon in icons)
        {
            if (icon.place == gameManager.Instance.currentPlace)
            {
                icon.icono.SetActive(true);
            }
            else
            {
                icon.icono.SetActive(false);
            }
        }
    }
}

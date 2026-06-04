using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gameCore : MonoBehaviour
{
    [Header("Script References")]
    public dialogueManager DM;
    public audioManager AU;
    public characterManager CM;
    public dialogueHistory DH;
    public uiManager UM;
    public inventoryInteraction II;

    [HideInInspector] public bool inventoryIsAvaibale;
    [HideInInspector] public bool mapIsObtained;
    [HideInInspector] public bool showVisura;
    [HideInInspector] public bool showInfoWindow;
    [HideInInspector] public bool isCinematic = false;
    [HideInInspector] public bool showDoor = false;
    [HideInInspector] public bool clickDoor = false;
    [HideInInspector] public bool stopMusic = false;
    [HideInInspector] public bool removeCharacter = false;


    void Start()
    {
        switch (gameManager.Instance.miniGameName)
        {
            case "Card Game": DM.StartScene("arrabalSanRoque_presenteAfterMiniGame"); break;
            case "Mario Game": DM.StartScene("arrabalSanRoque_pasadoAfterMiniGame"); break;
            case "Simon Says": DM.StartScene("arrabalLoreto_presenteAfterMiniGame"); break;
            case "Crochet": DM.StartScene("arrabalLoreto_pasadoAfterMiniGame"); break;
            case "FNF": DM.StartScene("arrabalSanJuan_presenteAfterMiniGame"); break;
            case "Undertale": DM.StartScene("arrabalSanJuan_pasadoAfterMiniGame"); break;
            case "ClearVisuras": DM.StartScene("ayun_returnAfterMiniGame"); break;
            default: DM.StartScene("intro_vistabella"); break;
        }
    }
}

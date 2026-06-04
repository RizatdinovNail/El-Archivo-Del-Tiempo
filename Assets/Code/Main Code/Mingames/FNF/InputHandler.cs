using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    PlayerControls playerControls;
    BeatGame beatGame;

    public List<NoteObject> totalNotes;
    public float hitWindow = 0.22f;
    int index = 0;


    void Awake()
    {
        playerControls = new PlayerControls();
        beatGame = FindFirstObjectByType<BeatGame>();
    }

    void OnEnable()
    {
        playerControls.Enable();

        playerControls.Player.Left.performed += _ => OnLanePressed(0);
        playerControls.Player.Up.performed += _ => OnLanePressed(2);
        playerControls.Player.Down.performed += _ => OnLanePressed(1);
        playerControls.Player.Right.performed += _ => OnLanePressed(3);
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    void OnLanePressed(int lane)
    {
        if (beatGame.totalNotes.Count == 0)
            return;

        // wrong arrow
        if (beatGame.topNote.lane != lane)
        {
            beatGame.topNote.Miss(true);
            return;
        }

        float songTime = (float)beatGame.SongTime;
        float hitWindow = 0.22f;

        if (Mathf.Abs(songTime - (float)beatGame.topNote.hitTime) <= hitWindow)
        {
            if(index == 0){
                index = 1;
            }
            else{
                index = 0;
            }

            beatGame.player.GetComponent<Image>().sprite = beatGame.playerSprites[beatGame.animationPlayer].animation[index];
            beatGame.player.GetComponent<Image>().SetNativeSize();
            beatGame.topNote.Hit();
            beatGame.NPC.GetComponent<Image>().sprite = beatGame.defaultNPC;
        }
    }

}

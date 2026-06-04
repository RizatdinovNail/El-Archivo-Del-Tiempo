using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NoteObject : MonoBehaviour
{
    public double hitTime;
    public float speed;
    public bool mustHit;
    public bool hit = false;
    public int lane;
    int index;

    private BeatGame beatGame;


    void Start()
    {
        beatGame = FindFirstObjectByType<BeatGame>();
        if (mustHit) {
            beatGame.totalNotes.Add(this);
            beatGame.NPCstarted = false;
            if(!beatGame.playerStarted)
            {
                if(beatGame.animationPlayer == 0) beatGame.animationPlayer = 1;
                else beatGame.animationPlayer = 0;
            }
            beatGame.playerStarted = true;
        }

        else{
            beatGame.playerStarted = false;
            if(!beatGame.NPCstarted)
            {
                if(beatGame.animationNPC == 0) beatGame.animationNPC = 1;
                else beatGame.animationNPC = 0;
            }
            beatGame.NPCstarted = true;
        }
        if (beatGame.topNote == null && mustHit) beatGame.topNote = this;
        beatGame.totalNotes.Sort((a, b) => a.hitTime.CompareTo(b.hitTime));
    }

    void Update()
    {
        float songTime = (float)beatGame.SongTime;

        transform.localPosition += new Vector3(0, speed * Time.deltaTime, 0);

        if (!hit && !mustHit && songTime > hitTime + 0.22f)
        {
            beatGame.player.GetComponent<Image>().sprite = beatGame.defaultPlayer;
            beatGame.player.GetComponent<Image>().SetNativeSize();
            Image npcImage = beatGame.NPC.GetComponent<Image>();
            if(beatGame.NPCframe == 0){
                beatGame.NPCframe = 1;
            }
            else{
                beatGame.NPCframe = 0;
            }

            npcImage.sprite = beatGame.npcSprites[beatGame.animationNPC].animation[beatGame.NPCframe];
            npcImage.SetNativeSize();
            Destroy(gameObject);
        }

        if (!hit && mustHit && songTime > hitTime + 0.30f) Miss(false);
    }

    public void Hit()
    {
        if (hit) return;
        hit = true;
        beatGame.score += 100;

        beatGame.totalNotes.Remove(this);
        Destroy(gameObject);
        if (beatGame.totalNotes.Count != 0)
        {
            beatGame.topNote = beatGame.totalNotes[0];
        }
    }

    public void Miss(bool wrongLane)
    {
        beatGame.score = Mathf.Max(0, beatGame.score - 25);
        if (!wrongLane)
        {
            beatGame.totalNotes.Remove(this);
            Destroy(gameObject);
            if (beatGame.totalNotes.Count != 0)
            {
                beatGame.topNote = beatGame.totalNotes[0];
            }
        }
    }
}
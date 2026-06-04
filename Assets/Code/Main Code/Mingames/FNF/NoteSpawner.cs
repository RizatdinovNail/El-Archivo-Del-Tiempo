using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class spawnPoints
{
    public Transform transform;
    public GameObject arrow;
    public Quaternion rotation;
}

public class NoteSpawner : MonoBehaviour
{
    public List<spawnPoints> spawnPoint = new List<spawnPoints>();
    public List<spawnPoints> NpcSpawnPoints = new List<spawnPoints>();
    public GameObject notePrefab;       // Prefab to spawn
    public float noteTravelTime = 1.0f; // Time before hit


    private BeatGame beatGame;
    private List<NoteData> notes = new List<NoteData>();
    private int nextNoteIndex = 0;
    private float distance = 445f * 2;
    private float width = 160f;
    private float height = 100f;
    private int noteName = 0;

    void Start()
    {
        beatGame = FindFirstObjectByType<BeatGame>();
        notes = beatGame.chart.notes;   // Load notes from chart
    }

    void Update()
    {
        if (!beatGame.gameIsStarted) return;
        if (!beatGame.music.isPlaying) return;

        double currentTime = beatGame.SongTime;

        while (nextNoteIndex < notes.Count &&
               notes[nextNoteIndex].time - noteTravelTime <= currentTime)
        {
            Spawn(notes[nextNoteIndex]);
            nextNoteIndex++;
        }
    }

    void Spawn(NoteData data)
    {
        GameObject obj = null;
        if (!data.mustHit)
        {
            obj = Instantiate(
                notePrefab,
                NpcSpawnPoints[data.lane].transform.position,
                Quaternion.identity,
                NpcSpawnPoints[data.lane].transform
            );

            obj.transform.rotation = NpcSpawnPoints[data.lane].rotation;
            obj.GetComponent<Image>().sprite = NpcSpawnPoints[data.lane].arrow.GetComponent<Image>().sprite;
        }

        else
        {
            obj = Instantiate(
            notePrefab,
            spawnPoint[data.lane].transform.position,
            Quaternion.identity,
            spawnPoint[data.lane].transform
            );
            obj.name = noteName.ToString();
            noteName++;
            obj.transform.rotation = spawnPoint[data.lane].rotation;
            obj.GetComponent<Image>().sprite = spawnPoint[data.lane].arrow.GetComponent<Image>().sprite;
        }

        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        var note = obj.GetComponent<NoteObject>();
        note.hitTime = data.time;
        note.speed = distance / noteTravelTime;
        note.lane = data.lane;
        note.mustHit = data.mustHit;
    }
}

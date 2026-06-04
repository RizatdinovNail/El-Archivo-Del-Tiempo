using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class dialogueFiles
{
    public TextAsset textFile;
    public Sprite background;
    [Header("Characters Used In This Scene")]
    public List<DialogueCharacterData> characters;

    [Header("Music")]
    public AudioClip backgroundMusic;
    public float volume;
    public AudioClip SFXeffect;
}

public class DialogueDatabase : MonoBehaviour
{
    public List<dialogueFiles> files;
    private Dictionary<string, DialogueScene> scenes =
        new Dictionary<string, DialogueScene>();

    void Awake()
    {
        LoadAllDialogues();
    }

    void LoadAllDialogues()
    {
        scenes.Clear();

        foreach (var file in files)
        {
            DialogueScene scene =
                JsonUtility.FromJson<DialogueScene>(file.textFile.text);

            if (scene == null || string.IsNullOrEmpty(scene.sceneId))
            {
                Debug.LogError($"Invalid dialogue file: {file.textFile.name}");
                continue;
            }

            // Inject Inspector data
            scene.background = file.background;
            scene.characters = file.characters;
            scene.backgroundMusic = file.backgroundMusic;
            scene.volume = file.volume;
            scene.SFXeffect = file.SFXeffect;

            scenes[scene.sceneId] = scene;
        }
    }


    public DialogueScene GetScene(string sceneId)
    {
        return scenes.TryGetValue(sceneId, out var scene)
            ? scene
            : null;
    }
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


[Serializable]
public class DialogueLine
{
    public string key;
    public string speaker;
    public string text;
    public string expression;
    public bool mapIsObtained;
    public bool inventoryIsUsable;
    public bool showVisura;
    public bool showInfoWindow;
    public bool showDoor;
    public bool clickDoor;
    public bool isChoicePoint;
    public DialogueChoice choice;
    public bool isCorrect = true;
    public int returnChoiceIndex = -1;
    public bool returnToChoice = false;
    public bool firstTimeChoice;
    public bool stopMusic = false;
    public bool removeCharacter = false;
}

[Serializable]
public class DialogueScene
{
    public string sceneId;
    public List<DialogueLine> dialogues = new List<DialogueLine>();
    [System.NonSerialized]
    public Sprite background;

    [Header("Characters Used In This Scene")]
    public List<DialogueCharacterData> characters;

    [Header("Music")]
    public AudioClip backgroundMusic;
    public float volume;
    public AudioClip SFXeffect;
}

[Serializable]
public class Animations
{
    public string expression;
    public List<Sprite> frames;
    public float frameRate = 7f;
}

[System.Serializable]
public class DialogueCharacterData
{
    public string characterName;

    [Header("Default Portrait")]
    public Sprite defaultSprite;

    [Header("Animations")]
    public List<Animations> animations;
}

[Serializable]
public class VisuraData
{
    public string description;
}

[System.Serializable]
public class DialogueChoice
{
    public string questionText;
    public List<DialogueOption> options;
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public int nextDialogueIndex = -1;
    public bool isCorrect = true;
}
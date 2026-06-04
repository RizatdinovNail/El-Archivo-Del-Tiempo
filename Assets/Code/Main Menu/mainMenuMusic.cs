using UnityEngine;

public class mainMenuMusic : MonoBehaviour
{
    public AudioSource audio;

    void Update()
    {
        audio.volume = gameManager.Instance.currentVolume;
    }
}

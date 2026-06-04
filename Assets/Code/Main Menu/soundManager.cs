using UnityEngine;
using UnityEngine.UI;

public class soundManager : MonoBehaviour
{

    public Slider volumeSlider;
    public Image soundSprite;
    public Sprite mute;
    public Sprite unMute;

    private float savedSoundVolume = 0f;

    void Start()
    {
        volumeSlider.value = gameManager.Instance.currentVolume;
    }
    void Update()
    {
        if (volumeSlider.value == 0)
        {
            soundSprite.sprite = mute;
        }

        else
        {
            soundSprite.sprite = unMute;
        }
    }

    public void SetVolume()
    {
        gameManager.Instance.currentVolume = volumeSlider.value;
    }

    public void clickSoundButton()
    {
        if (soundSprite.sprite == unMute)
        {
            savedSoundVolume = volumeSlider.value;
            volumeSlider.value = 0f;
        }

        else
        {
            volumeSlider.value = savedSoundVolume;
            savedSoundVolume = 0f;
        }
        SetVolume();
    }
}

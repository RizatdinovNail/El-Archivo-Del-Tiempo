using UnityEngine;

public class clickSound : MonoBehaviour
{
    public AudioSource click;

    void Update()
    {
        click.volume = 1 * gameManager.Instance.currentVolume;
    }
    public void playClick()
    {
        click.Play();
    }
}

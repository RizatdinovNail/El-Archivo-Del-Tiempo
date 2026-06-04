using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicio : MonoBehaviour
{
    public AudioSource sonidoJugar;

    void Start()
    {
        sonidoJugar = GetComponent<AudioSource>();
        sonidoJugar.volume = sonidoJugar.volume * gameManager.Instance.currentVolume;
    }

    public void OnStartClick()
    {
        sonidoJugar.Play();
        SceneManager.LoadScene("SampleScene");
    }

    public void Exitgame()
    {
        gameManager.Instance.wasInSR = true;
        gameManager.Instance.miniGameName = "Mario Game";
        SceneManager.LoadScene("Game Scene");
    }
}

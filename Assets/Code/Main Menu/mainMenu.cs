using UnityEngine;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour
{
    public Button continueButton;
    public Button loadButton;
    public void clickButton(Button btn)
    {
        switch (btn.name)
        {
            case "Continue":
                {
                    Debug.Log("Continue"); break;
                }
            case "New Game": ScreenFader.Instance.TransitionToScene("Game Scene", 0.2f, 0.2f); break;
            case "Load Game": Debug.Log("Load Game"); break;
            case "Settings": ScreenFader.Instance.TransitionToScene("settings", 0.2f, 0.2f); break;
            case "Credits": ScreenFader.Instance.TransitionToScene("credits", 0.2f, 0.2f); break;
            case "Exit": Application.Quit(); break;
            case "Back Button": ScreenFader.Instance.TransitionToScene("Main Menu", 0.2f, 0.2f); break;
        }
    }

    void Update()
    {
        if (gameManager.Instance.saveList.Count == 0)
        {
            continueButton.interactable = false;
            loadButton.interactable = false;
        }

        else
        {
            continueButton.interactable = true;
            loadButton.interactable = true;
        }
    }

}

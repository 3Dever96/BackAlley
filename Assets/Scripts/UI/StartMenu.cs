using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    private void Start()
    {
        Button button = GetComponentInChildren<Button>();

        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }

    public void StartBattle()
    {
        SceneController.instance.LoadNewScene("SCN_Arena");
    }

    public void Options()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

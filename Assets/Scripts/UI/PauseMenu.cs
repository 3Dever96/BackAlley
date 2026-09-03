using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject background;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        BattleManager.instance.OnPause += OnPause;
    }

    private void OnDisable()
    {
        BattleManager.instance.OnPause -= OnPause;
    }

    public void OnPause()
    {
        background.SetActive(true);
        resumeButton.interactable = true;
        quitButton.interactable = true;
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void Resume()
    {
        EventSystem.current.SetSelectedGameObject(null);

        resumeButton.interactable = false;
        quitButton.interactable = false;
        background.SetActive(false);

        BattleManager.instance.state = BattleState.Fight;

        Time.timeScale = 1f;
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneController.instance.LoadNewScene("SCN_StartScreen");
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class KnockoutController : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text victoryText;
    [SerializeField] private GameObject texts;
    [SerializeField] private GameObject buttons;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button rematchButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Image background;
    private float a;

    [SerializeField] private string nextScene;

    private void OnEnable()
    {
        BattleManager.instance.OnVictory += ShowBattleResults;
    }

    private void OnDisable()
    {
        BattleManager.instance.OnVictory -= ShowBattleResults;
    }

    public void ShowBattleResults()
    {
        StartCoroutine(FadeIn());
    }

    private void ActivateMenu()
    {
        texts.SetActive(true);
        buttons.SetActive(true);

        if (BattleManager.instance.state == BattleState.Victory)
        {
            if (BattleManager.instance.PlayerWon())
            {
                victoryText.text = "You Win!";
                continueButton.gameObject.SetActive(true);
                continueButton.interactable = true;
                EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
            }
            else
            {
                victoryText.text = "You Lose!";
                rematchButton.gameObject.SetActive(true);
                rematchButton.interactable = true;
                EventSystem.current.SetSelectedGameObject(rematchButton.gameObject);
            }

            quitButton.interactable = true;
        }
    }

    private IEnumerator FadeIn()
    {
        a = 0f;

        while (a < 0.3f)
        {
            a += Time.unscaledDeltaTime * 0.1f;

            Color color = new Color(0f, 0f, 0f, a);

            background.color = color;

            yield return null;
        }

        ActivateMenu();
    }

    public void Continue()
    {
        // SceneController.instance.LoadNewScene(nextScene);
    }

    public void Rematch()
    {
        SceneController.instance.LoadNewScene(SceneController.instance.currentScene);
    }

    public void QuitButton()
    {
        SceneController.instance.LoadNewScene("SCN_StartScreen");
    }
}

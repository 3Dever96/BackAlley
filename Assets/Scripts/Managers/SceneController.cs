using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed;
    private float alpha;

    public string currentScene;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);

        LoadNewScene("SCN_StartScreen");
    }

    public void LoadNewScene(string sceneName)
    {
        StartCoroutine(TransitionToScene(sceneName));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        // ==========================================
        // PHASE 1: FADE TO BLACK (Curtain Closes)
        // ==========================================
        alpha = 0f;

        // This loop runs cleanly until the curtain is 100% pitch black
        while (alpha < 1f)
        {
            alpha += Time.unscaledDeltaTime * fadeSpeed;
            fadeImage.color = new Color(0f, 0f, 0f, Mathf.Min(alpha, 1f));
            yield return null; // Wait for the next engine frame
        }

        alpha = 1f; // Clamp to absolute black safely outside the loop

        // ==========================================
        // PHASE 2: THE LOADING GATE (Trigger Once)
        // ==========================================
        // Fire the async loading line EXACTLY ONCE while the screen is solid black.
        // This hides all scene spawning lag completely from the player's eyes!
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneName);

        // THE WAITING ANCHOR: This line beautifully pauses your coroutine thread.
        // It tells Unity: "Do absolutely nothing else in this script until the 
        // scene background loading data hits 100% completion!"
        while (!sceneLoad.isDone)
        {
            yield return null;
        }

        currentScene = sceneName;

        // ==========================================
        // PHASE 3: FADE TO VISUALS (Curtain Opens)
        // ==========================================
        // The scene is now loaded! Run your clean, isolated fade-in loop.
        while (alpha > 0f)
        {
            alpha -= Time.unscaledDeltaTime * fadeSpeed;
            fadeImage.color = new Color(0f, 0f, 0f, Mathf.Max(alpha, 0f));
            yield return null;
        }

        alpha = 0f; // Clamp cleanly to absolute transparent baseline
    }

}

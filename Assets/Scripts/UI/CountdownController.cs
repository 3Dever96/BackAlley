using UnityEngine;

public class CountdownController : MonoBehaviour
{
    [System.Serializable]
    private class CountdownText
    {
        public string text;
        public Color color;
    }

    [SerializeField] TMPro.TMP_Text visual;
    [SerializeField] private CountdownText[] text;
    private int index;
    private int lastIndex;
    private float a = 1f;

    private void Update()
    {
        if (BattleManager.instance.state == BattleState.Start)
        {
            index = Mathf.CeilToInt(BattleManager.instance.countdown);

            visual.text = text[index].text;
            Color newColor = text[index].color;

            newColor.a = a;

            visual.color = newColor;

            a -= Time.deltaTime * 0.5f;

            if (lastIndex != index)
            {
                lastIndex = index;
                a = 1f;
            }
        }
        else
        {
            index = 0;

            if (lastIndex != index)
            {
                lastIndex = index;
                a = 1f;
            }

            visual.text = text[index].text;
            Color newColor = text[index].color;

            newColor.a = a;

            visual.color = newColor;

            a -= Time.deltaTime;

            if (a <= 0f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}

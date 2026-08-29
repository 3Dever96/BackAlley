using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    [SerializeField] private int aiFighterCount;

    public BattleState state;

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
    }

    public void ChangeState(BattleState newState)
    {
        state = newState;
    }

    public void AddAIFighter()
    {
        ChangeState(BattleState.Start);

        aiFighterCount++;

        StopCoroutine(StartCountdown());

        StartCoroutine(StartCountdown());
    }

    public void RemoveAIFighter()
    {
        aiFighterCount--;

        if (aiFighterCount == 0)
        {
            ChangeState(BattleState.Victory);
        }
    }

    private IEnumerator StartCountdown()
    {
        float countdown = 3f;

        while (countdown > 0)
        {
            countdown -= Time.deltaTime;

            yield return null;
        }

        if (countdown <= 0f)
        {
            ChangeState(BattleState.Fight);
        }
    }
}

public enum BattleState
{
    Start,
    Fight,
    Victory,
    GameOver,
    Paused
}

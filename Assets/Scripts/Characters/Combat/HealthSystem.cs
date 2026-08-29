using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    private bool isBot;

    [SerializeField] private float maxHealth;
    public float currentHealth;

    public event Action OnSpawn;
    public event Action<int> OnHealthChanged;
    public event Action OnKnockout;

    private void Start()
    {
        currentHealth = maxHealth;

        OnSpawn?.Invoke();
        OnHealthChanged?.Invoke(Mathf.RoundToInt(currentHealth));
    }

    public void SetBot(bool value)
    {
        isBot = value;

        if (isBot)
        {
            BattleManager.instance.AddAIFighter();
            OnKnockout += BattleManager.instance.RemoveAIFighter;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

        OnHealthChanged?.Invoke(Mathf.RoundToInt(currentHealth));

        if (currentHealth == 0)
        {
            OnKnockout?.Invoke();
        }
    }
}

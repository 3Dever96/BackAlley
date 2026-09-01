using UnityEngine;
using System; // Required to unlock C# Actions and Event delegate signatures

/// <summary>
/// Manages life-cycle health pools, registers structural character damage,
/// and broadcasts condition data updates up to the user interface and match managers.
/// </summary>
public class HealthSystem : MonoBehaviour
{
    // ==========================================
    // TRACKING COMPONENT CONFIGURATIONS
    // ==========================================
    private bool isBot;                         // Flag determining if this entity is driven by an AI script

    [SerializeField] private float maxHealth;   // The unchanging cap limit value of the core vitality tank
    public float currentHealth;                 // The running mutable scalar tracking surviving hitpoints

    // ==========================================
    // INSTANCE DATA BROADCAST EVENT CHANNELS
    // ==========================================
    public event Action OnSpawn;                // Fires the immediate frame the instance wakes up in the match arena
    public event Action<int> OnHealthChanged;   // Fires whenever health values fluctuate, passing the new rounded integer
    public event Action OnKnockout;             // Fires precisely the single frame currentHealth reaches absolute zero

    private void Start()
    {
        // Initialize our health tank to max capacity at boot
        currentHealth = maxHealth;

        // 1. BROADCAST INITIALIZATION STATUS
        OnSpawn?.Invoke();
        OnHealthChanged?.Invoke(Mathf.RoundToInt(currentHealth));
    }

    /// <summary>
    /// Subtracts incoming strike variables from the active life pool and evaluates defeat threshold boundaries.
    /// Uses architectural messages to route physics packages down into adjacent component states.
    /// </summary>
    /// <param name="data">The self-contained AttackData package containing spatial direction and ScriptableObject move details.</param>
    public void TakeDamage(AttackData data)
    {
        // Protect bounds: clip values tightly between zero floor and maximum capabilities ceilings
        // We pull the raw attackPow variable directly from the packaged AttackMove ScriptableObject asset
        currentHealth = Mathf.Clamp(currentHealth - data.attack.attackPow, 0, maxHealth);

        // INTERNAL HUB BROADCAST: Fires an internal message hook down to all sibling components.
        // This is a brilliant, highly decoupled way to notify your state controllers or AI brain 
        // to invoke methods like "OnHit(data)" without creating rigid, hard-coded dependencies!
        BroadcastMessage("OnHit", data);

        // Alert subscribing UI sliders or sound audio players that structural damage occurred
        OnHealthChanged?.Invoke(Mathf.RoundToInt(currentHealth));

        // 2. EVALUATE DEFEAT STATE
        if (currentHealth == 0)
        {
            // Unleash our local instance death sequence (Swaps animations, cleans counters)
            OnKnockout?.Invoke();
        }
    }
}

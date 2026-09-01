using UnityEngine;
using System.Collections; // Required to unlock Coroutines and IEnumerator asynchronous timers

/// <summary>
/// Architecture Role: Master Referee / Global Match Lifecycle Manager.
/// Implements a persistent Singleton pattern to track global match rules, 
/// manage active AI counts dynamically via sub-system hooks, and drive core state transitions.
/// </summary>
public class BattleManager : MonoBehaviour
{
    // ==========================================
    // PERSISTENT SINGLETON PATTERN INSTANCE
    // ==========================================
    public static BattleManager instance;

    // ==========================================
    // GLOBAL MATCH LOGISTICS & TRACKERS
    // ==========================================
    [Header("Match Progression Tracks")]
    [SerializeField] private int aiFighterCount; // Active counter tracking living computer adversaries in the arena

    public BattleState state;                    // Master switch tracking the active global timeline phase

    private void Awake()
    {
        // 1. SINGLETON SAFEGUARD PATTERN
        // Enforce that only one definitive manager instance can control the scene canvas rules at a time
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                // Destroy duplicates immediately if an accidental secondary manager spawns
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Master state router. Shifts the global gears of the match timeline framework (Start, Fight, Victory, etc.).
    /// </summary>
    /// <param name="newState">The incoming BattleState phase to assign globally.</param>
    public void ChangeState(BattleState newState)
    {
        state = newState;
    }

    /// <summary>
    /// Asynchronous match entry clock. Holds the scene gameplay updates frozen for 3 seconds 
    /// before dropping characters into active combat.
    /// </summary>
    private IEnumerator StartCountdown()
    {
        float countdown = 3f;

        // Count down smoothly using delta time intervals independent of hardware frame-rates
        while (countdown > 0)
        {
            countdown -= Time.deltaTime;
            yield return null; // Wait for the very next engine frame thread update before looping
        }

        // 4. COMBAT INITIATION TRIGGER
        if (countdown <= 0f)
        {
            // Drop the gates and unlock character update logic sweeps completely!
            ChangeState(BattleState.Fight);
        }
    }
}

/// <summary>
/// Architecture Role: Global Timeline Configuration Enum.
/// Explicit milestones tracking where the game loop is sitting at any single millisecond of execution.
/// </summary>
public enum BattleState
{
    Start,    // Match intro / Countdown phase. Locomotion loops are locked.
    Fight,    // Active gameplay simulation. Character updates and inputs run continuously.
    Knockout, // Temporarily slow down time to indicate the match is over.
    Victory,  // Show the winner celebrating the victory and show options for continuing, starting a rematch, or quitting.
    Paused    // Global simulation suspended via pause option menus.
}

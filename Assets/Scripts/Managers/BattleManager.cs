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
    /// Increments the master adversary count and refreshes the entry countdown. 
    /// Triggered upstream dynamically via HealthSystem.SetBot initialization spoke loops.
    /// </summary>
    public void AddAIFighter()
    {
        // Force the match into the loading/intro layout phase while entities spawn
        ChangeState(BattleState.Start);
        aiFighterCount++;

        // 2. TIMING RESET PROTECTION
        // Stop and restart the clock to ensure the 3-second countdown doesn't fire early 
        // if multiple AI prefabs instantiate across separate frames during loading sequences.
        StopCoroutine(StartCountdown());
        StartCoroutine(StartCountdown());
    }

    /// <summary>
    /// Decrements the active adversary counter and evaluates global win conditions. 
    /// Hooked up dynamically as an automated event delegate listener inside HealthSystem.
    /// </summary>
    public void RemoveAIFighter()
    {
        aiFighterCount--;

        // 3. WIN STATE EVALUATION
        // The exact frame all computer-driven enemies drop to absolute zero HP, crown the player champion!
        if (aiFighterCount == 0)
        {
            ChangeState(BattleState.Victory);

            // (Tomorrow: This is the exact condition slot where you can trigger your UI canvas overlay fading loops!)
        }
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
    Victory,  // Player wins! All enemies are KO'd. Triggers celebratory loops.
    GameOver, // Player loses! Health tank hits 0. Triggers defeat responses.
    Paused    // Global simulation suspended via pause option menus.
}

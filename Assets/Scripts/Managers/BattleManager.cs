using System; // Required to unlock Coroutines and IEnumerator asynchronous timers
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Dictionary<TeamColor, List<FighterController>> fighterTeam = new Dictionary<TeamColor, List<FighterController>>();
    private TeamColor playerTeam;

    public BattleState state;                    // Master switch tracking the active global timeline phase

    public event Action OnVictory;
    public event Action OnPause;

    public float countdown;

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

    public void AddFighter(FighterController newFighter)
    {
        if (!fighterTeam.ContainsKey(newFighter.team))
        {
            fighterTeam.Add(newFighter.team, new List<FighterController>());
        }

        fighterTeam[newFighter.team].Add(newFighter);

        if (newFighter.inputType == InputType.Player)
        {
            playerTeam = newFighter.team;
        }

        StopCoroutine(StartCountdown());
        StartCoroutine(StartCountdown());
    }

    // Inside your master BattleManager or Team Management hub:
    public void UpdateFighters()
    {
        int standingTeams = 0;

        // =========================================================================
        // 1. SYSTEMATIC TEAM ENUM ITERATION
        // =========================================================================
        // We loop through the master enum definition array to ensure we check every color 
        // in the exact same deterministic order every single time.
        foreach (TeamColor teamColor in System.Enum.GetValues(typeof(TeamColor)))
        {
            int standingFighters = 0;

            // 2. THE VALIDATION SAFETY GATE
            // Verify that the team key exists in your dictionary configuration 
            // to prevent unhandled reference crashes on empty team slots.
            if (fighterTeam.ContainsKey(teamColor) && fighterTeam[teamColor] != null)
            {
                // Cache the list pointer locally for optimization
                List<FighterController> activeTeamList = fighterTeam[teamColor];

                // =========================================================================
                // 3. THE INNER INDIVIDUAL FIGHTER ITERATION
                // =========================================================================
                // Now we step through every active character registered under this team color bracket
                for (int i = 0; i < activeTeamList.Count; i++)
                {
                    FighterController fighter = activeTeamList[i];

                    // Safety Check: Ensure the fighter instance hasn't been destroyed mid-match
                    if (fighter != null)
                    {
                        if (!fighter.IsKnockedOut)
                        {
                            standingFighters++;
                        }
                    }
                }
            }

            if (standingFighters > 0)
            {
                standingTeams++;
            }
        }

        if (standingTeams == 1)
        {
            ChangeState(BattleState.Knockout);
            StartCoroutine(KnockoutTime());
        }
    }

    public bool PlayerWon()
    {
        int standingFighters = 0;

        if (fighterTeam.ContainsKey(playerTeam) && fighterTeam[playerTeam] != null)
        {
            // Cache the list pointer locally for optimization
            List<FighterController> activeTeamList = fighterTeam[playerTeam];

            // =========================================================================
            // 3. THE INNER INDIVIDUAL FIGHTER ITERATION
            // =========================================================================
            // Now we step through every active character registered under this team color bracket
            for (int i = 0; i < activeTeamList.Count; i++)
            {
                FighterController fighter = activeTeamList[i];

                // Safety Check: Ensure the fighter instance hasn't been destroyed mid-match
                if (fighter != null)
                {
                    if (!fighter.IsKnockedOut)
                    {
                        standingFighters++;
                    }
                }
            }
        }

        if (standingFighters > 0)
        {
            return true;
        }

        return false;
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
        countdown = 3f;

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

    private IEnumerator KnockoutTime()
    {
        Time.timeScale = 0.15f;

        float knockoutTime = 3f;

        while (knockoutTime > 0f)
        {
            knockoutTime -= Time.unscaledDeltaTime;

            yield return null;
        }

        Time.timeScale = 1f;

        ChangeState(BattleState.Victory);
        OnVictory?.Invoke();
    }

    public void PauseGame()
    {
        OnPause?.Invoke();
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

public enum TeamColor
{
    Red,
    Blue,
    Green,
    Yellow
}

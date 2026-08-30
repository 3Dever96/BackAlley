using UnityEngine;

/// <summary>
/// Architecture Role: Locomotive State / Downed State.
/// Controls the hard knockdown/recovery sequence when a fighter is sent flying by a heavy impact.
/// Ground-anchors the character model, immobilizes movement components, and temporarily deactivates
/// body hitboxes to act as an un-interruptible downtime window before returning control to the player.
/// </summary>
[System.Serializable]
public class RecoveryState : MoveState
{
    // =========================================================================
    // RECOVERY TUNING PARAMETERS
    // =========================================================================
    [Header("Downtime Tuning")]
    [SerializeField] private float recoveryTime;  // Design configuration float tracking how long a fighter stays down
    private float currentTime;                    // Running countdown clock tracking active recovery frame progression

    [Header("Hitbox Overrides")]
    [SerializeField] private GameObject hitbox;   // Reference to the passive body hitbox object receiving enemy damage

    public override void StartState(FighterController fighter)
    {
        // 1. FLOOR ANCHOR & LOCK DOWN
        // Collapse horizontal movement completely and snap vertical speeds to ground deadzones
        fighter.CurrentSpeed = 0f;
        fighter.VerticalSpeed = fighter.stickForce;

        // 2. TIMING INITIALIZATION
        // Reset our clock container to full design specifications at entry
        currentTime = recoveryTime;

        // 3. MERCY SYSTEM PROTECTION
        // Shut down the main body hitbox trigger completely while the character lies flat.
        // This makes the character 100% invincible while on the floor, preventing opponents from spawn-trapping!
        if (hitbox != null)
        {
            hitbox.SetActive(false);
        }
    }

    public override void UpdateState(FighterController fighter)
    {
        // Ground frame updates sit completely silent here—which is exactly what we want for a downed character
    }

    public override void ChangeState(FighterController fighter)
    {
        // 4. TIMEOUT RECOVERY TRACKING
        // Keep the local countdown running continuously independent of frame rates
        currentTime -= Time.deltaTime;

        // Once the countdown clock crosses its floor limit, force the wake-up sequence
        if (currentTime <= 0f)
        {
            // Transition smoothly back into neutral ground navigation to restore active controller inputs
            fighter.SetState(fighter.GroundState);
        }
    }

    public override void ExitState(FighterController fighter)
    {
        // =========================================================================
        // 5. DEFENSIVE SHIELD RECOVERY
        // =========================================================================
        // The exact frame the character finishes their wake-up/get-up state animation loop,
        // re-activate the body hitbox mesh so they can take damage normally in active neutral neutral game windows!
        if (hitbox != null)
        {
            hitbox.SetActive(true);
        }
    }
}

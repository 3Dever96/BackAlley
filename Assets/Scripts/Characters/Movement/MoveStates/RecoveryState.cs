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
        // 4. DOWNED LOOP ANIMATION
        // Instantly force the character model skeleton to remain flat in its knocked-out posture.
        // Animancer loops this asset frame efficiently while the recovery clock ticks down.
        fighter.Animancer.Play(fighter.Anim.knockout, 0.15f);
    }

    public override void ChangeState(FighterController fighter)
    {
        // 5. TIMEOUT RECOVERY TRACKING
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
        // 6. DEFENSIVE SHIELD RECOVERY & WAKE-UP ANIMATION
        // =========================================================================
        // The exact frame the character finishes their wake-up/get-up state animation loop,
        // re-activate the body hitbox mesh so they can take damage normally in active neutral game windows!
        if (hitbox != null)
        {
            hitbox.SetActive(true);
        }

        // Engage our shared public property lock switch to block manual locomotion input checks inside GroundState
        fighter.IsRecovering = true;

        // TRIGGER WAKE-UP ANCHOR: As we break out of this state back into GroundState locomotion,
        // command the bones to snap cleanly into their "getting back up" recovery animation.
        var state = fighter.Animancer.Play(fighter.Anim.recover, 0.15f);

        // 7. VERSION 8.0+ OWNED EVENTS LAMBDA HANDSHAKE
        // The absolute split-second the recovery animation concludes its single play-through,
        // clear the central flag so your GroundState input protection loops instantly drop away!
        state.OwnedEvents.OnEnd = () =>
        {
            fighter.IsRecovering = false;
        };
    }
}

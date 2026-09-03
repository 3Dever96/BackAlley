using Animancer;
using UnityEngine;

/// <summary>
/// Architecture Role: Locomotive State.
/// Manages standard ground navigation, processing user running direction vectors,
/// tap-tempo jump executions, and initiating the universal combat combo entry sequence.
/// </summary>
[System.Serializable]
public class GroundState : MoveState
{
    // ==========================================
    // TUNING CONFIGURATIONS
    // ==========================================
    [Header("Locomotion Tuning")]
    [SerializeField] private float moveSpeed;    // Max speed scalar applied when running
    [SerializeField] private float jumpSpeed;    // Immediate upward velocity applied on jump frame

    // ==========================================
    // INPUT TAP-VALIDATION GUARDS
    // ==========================================
    private bool canJump;                       // Safety flag forcing button release before another jump
    private bool canAttack;                     // Safety flag forcing button release before entering AttackState

    public override void StartState(FighterController fighter)
    {
        // 1. FLOOR ANCHOR: Snap downward tracking forces to flat floor tolerances
        fighter.VerticalSpeed = fighter.stickForce;

        // Prime tap input toggles to evaluate fresh key hold conditions
        canJump = false;
        canAttack = false;

        // 2. STATE ENTRY SAFETY CHECK
        // If the Animancer engine has completely stopped or is waking up for the first time, 
        // immediately blend into our default idle stance over a clean 0.15-second crossfade.
        if (!fighter.Animancer.IsPlaying())
        {
            fighter.Animancer.Play(fighter.Anim.idle, 0.15f);
        }
    }

    public override void UpdateState(FighterController fighter)
    {
        // =========================================================================
        // 3. WAKE-UP PROTECTION GATING
        // =========================================================================
        // Check our shared public property lock switch. If the character is currently 
        // rising from a knockdown, bypass input animation swaps entirely to protect the wake-up clip.
        if (!fighter.IsRecovering)
        {
            // =========================================================================
            // 4. HORIZONTAL DIRECTIONAL LOCOMOTION & SNAP CLIPS
            // =========================================================================
            if (fighter.Input.direction != Vector3.zero)
            {
                // Apply our design running speed to the hub variables
                fighter.CurrentSpeed = moveSpeed;

                // DIRECT SNAP SWITCH: Forces the model instantly into its running cycle. 
                // Animancer crossfades this over 0.15 seconds, creating a snappy but fluid acceleration.
                fighter.Animancer.Play(fighter.Anim.run, 0.15f);
            }
            else
            {
                // No input detected: collapse horizontal speed components to a dead halt
                fighter.CurrentSpeed = 0f;

                // DIRECT SNAP SWITCH: Instantly returns the skeletal bones back to our idle sway posture.
                fighter.Animancer.Play(fighter.Anim.idle, 0.15f);
            }
        }

        // 5. MESH ORIENTATION CALCULATIONS
        // Continuously rotate our character model mesh to snap toward its active movement heading.
        // Inline validation safety door: Only update the physical direction target if the thumbstick 
        // vector is actively pressed, preserving the last faced angle when the joystick snaps to neutral.
        fighter.Direction = fighter.Input.direction != Vector3.zero ? fighter.Input.direction : fighter.Direction;
        fighter.FaceDirection(fighter.Direction, fighter.turnSpeed);

        // =========================================================================
        // 6. JUMP INPUT REGISTER
        // =========================================================================
        // Trigger upward velocity strictly if they tapped the jump button (and let go previously)
        if (fighter.Input.jump && canJump)
        {
            fighter.VerticalSpeed = jumpSpeed;
        }

        // Anti-Mashing Property Logic: Force button releases between jump executions
        canJump = !fighter.Input.jump;
    }

    public override void ChangeState(FighterController fighter)
    {
        // 7. AIR STATE TRANSITION EVALUATION
        // Shift out of ground configurations into AirState if an upward force pushes us,
        // or if our environmental raycast sphere verifies there is no solid floor beneath our feet
        if (fighter.VerticalSpeed > 0f || !fighter.CheckCollision(fighter.transform.position, Vector3.up))
        {
            fighter.SetState(fighter.AirState);
            return; // Break execution frame path to prevent simultaneous state assignment collisions
        }

        // 8. ATTACK STATE TRANSITION EVALUATION
        // Shift out of navigation loops directly into combat arrays if attack button is cleanly tapped
        if (fighter.Input.attack && canAttack)
        {
            fighter.SetState(fighter.AttackState);
            return;
        }

        // Anti-Mashing Property Logic: Enforce strict tap cadence for combo opening hits
        canAttack = !fighter.Input.attack;
    }

    public override void ExitState(FighterController fighter)
    {
        // Standard locomotion cleanup code parameters sit here if expanding later
    }
}

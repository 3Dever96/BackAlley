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

    [SerializeField] private AnimationClip idle;
    [SerializeField] private AnimationClip run;

    public override void StartState(FighterController fighter)
    {
        // 1. FLOOR ANCHOR: Snap downward tracking forces to flat floor tolerances
        fighter.VerticalSpeed = fighter.stickForce;

        // Prime tap input toggles to evaluate fresh key hold conditions
        canJump = false;
        canAttack = false;

        if (!fighter.Animancer.IsPlaying())
        {
            fighter.Animancer.Play(idle, 0.15f);
        }
    }

    public override void UpdateState(FighterController fighter)
    {
        // 2. HORIZONTAL DIRECTIONAL LOCOMOTION
        if (fighter.Input.direction != Vector3.zero)
        {
            // Apply our design running speed to the hub variables
            fighter.CurrentSpeed = moveSpeed;
            fighter.Direction = fighter.Input.direction;
            fighter.Animancer.Play(run, 0.15f);
        }
        else
        {
            // No input detected: collapse horizontal speed components to a dead halt
            fighter.CurrentSpeed = 0f;
            fighter.Animancer.Play(idle, 0.15f);
        }

        // Continuously rotate our character model mesh to snap toward its active movement heading
        fighter.FaceDirection(fighter.Direction, fighter.turnSpeed);

        // 3. JUMP INPUT REGISTER
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
        // 4. AIR STATE TRANSITION EVALUATION
        // Shift out of ground configurations into AirState if an upward force pushes us,
        // or if our environmental raycast sphere verifies there is no solid floor beneath our feet
        if (fighter.VerticalSpeed > 0f || !fighter.CheckCollision(fighter.transform.position, Vector3.up))
        {
            fighter.SetState(fighter.AirState);
            return; // Break execution frame path to prevent simultaneous state assignment collisions
        }

        // 5. ATTACK STATE TRANSITION EVALUATION
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

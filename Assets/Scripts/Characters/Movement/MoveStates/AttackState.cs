using UnityEngine;

/// <summary>
/// Architecture Role: Locomotive State / Combat Execution State.
/// Drives the universal single-state combo engine. Handles dynamic timing countdowns, 
/// processes the precise canAttack button-release requirements, scales the physical hurtbox bounds,
/// and re-enters itself via the hub to advance through the 3-hit string.
/// </summary>
[System.Serializable]
public class AttackState : MoveState
{
    // ==========================================
    // COMBAT RUNTIME TRACKERS
    // ==========================================
    private AttackMove currentAttack;  // Local data cartridge cache for the active combo step statistics
    private bool didAttack;            // Flag confirming a valid button link occurred inside the input window
    private float currentTime;         // Running execution clock timer tracker for active frame data
    private bool canAttack;            // Safety flag ensuring players release the key before pressing again

    public override void StartState(FighterController fighter)
    {
        // 1. Immobilize movement during the active attack frames
        fighter.CurrentSpeed = 0f;
        fighter.VerticalSpeed = 0f;

        // 2. Fetch data based on the state we JUST left
        currentAttack = fighter.Attack.GetCurrentAttack(fighter.LastState);

        // Reset variables cleanly at the start of EVERY new combo hit
        didAttack = false;
        canAttack = false;
        currentTime = 0f;

        if (currentAttack != null)
        {
            fighter.Hurtbox.ActivateHitBox(currentAttack);

            fighter.Animancer.Play(currentAttack.attackAnimation, 0.15f);
        }
        else
        {
            // Fail-safe: if no move is found, drop safely back to locomotion
            fighter.SetState(fighter.LastState);
        }
    }

    public override void UpdateState(FighterController fighter)
    {
        currentTime += Time.deltaTime;

        // 3. COMBO BUFFER WINDOW EVALUATION
        // Once the timer passes the move's custom buffer frame, open the window!
        if (currentTime >= currentAttack.inputBuffer)
        {
            // If they press the button anywhere inside this window, cache the queue
            if (fighter.Input.attack && canAttack)
            {
                didAttack = true;
                fighter.SetState(this);
            }

            // Anti-Mashing Property Logic: Enforce strict tap cadence by verifying key releases
            canAttack = !fighter.Input.attack;
        }
    }

    public override void ChangeState(FighterController fighter)
    {
        // 4. ANIMATION STATE CONCLUSION TIMEOUT
        if (currentTime >= currentAttack.attackDuration)
        {
            // Dropped combo: Return to neutral locomotion safely
            fighter.SetState(fighter.LastState);
        }
    }

    public override void ExitState(FighterController fighter)
    {
        // Shut down the dangerous damage box when moving between actions
        fighter.Hurtbox.DeactivateHitBox();

        // CONDITIONAL COMBO RESET: Only clear the index back to 0 if the player
        // dropped the combo timing window. If they are successfully advancing,
        // DO NOT wipe the combo deck!
        if (!didAttack)
        {
            fighter.Attack.ResetCombo();
        }
    }
}

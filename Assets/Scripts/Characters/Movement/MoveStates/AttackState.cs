using UnityEngine;

[System.Serializable]
public class AttackState : MoveState
{
    private AttackMove currentAttack;

    private bool didAttack;
    private float currentTime;
    private bool canAttack;

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
            }

            canAttack = !fighter.Input.attack;
        }
    }

    public override void ChangeState(FighterController fighter)
    {
        // 4. ANIMATION STATE CONCLUSION TIMEOUT
        if (currentTime >= currentAttack.attackDuration)
        {
            if (!didAttack)
            {
                // Dropped combo: Return to neutral locomotion safely
                fighter.SetState(fighter.LastState);
            }
            else
            {
                // Successful link! Re-enter this EXACT state safely via the Hub.
                // SetState will call ExitState() -> update LastState -> trigger StartState() 
                // allowing your components to cycle smoothly to Punch 2/3!
                fighter.SetState(this);
            }
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

using UnityEngine;

public class FinalState : MoveState
{
    public override void StartState(FighterController fighter)
    {
        fighter.CurrentSpeed = 0f;
        fighter.VerticalSpeed = fighter.stickForce;

        if (!fighter.IsKnockedOut)
        {
            fighter.Animancer.Play(fighter.Anim.victory);
        }
        else
        {
            fighter.Animancer.Play(fighter.Anim.knockout);
        }
    }

    public override void UpdateState(FighterController fighter)
    {
        
    }

    public override void ChangeState(FighterController fighter)
    {
        
    }

    public override void ExitState(FighterController fighter)
    {
        
    }
}

using UnityEngine;

public class ReadyState : MoveState
{
    public override void StartState(FighterController fighter)
    {
        fighter.Animancer.Play(fighter.Anim.idle);
    }

    public override void UpdateState(FighterController fighter)
    {
        
    }

    public override void ChangeState(FighterController fighter)
    {
        if (BattleManager.instance.state == BattleState.Fight)
        {
            fighter.SetState(fighter.GroundState);
        }
    }

    public override void ExitState(FighterController fighter)
    {
        
    }
}

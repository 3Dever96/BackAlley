using UnityEngine;

public abstract class MoveState
{
    public abstract void StartState(FighterController fighter);
    public abstract void UpdateState(FighterController fighter);
    public abstract void ChangeState(FighterController fighter);
    public abstract void ExitState(FighterController fighter);
}

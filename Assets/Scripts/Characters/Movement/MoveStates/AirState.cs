using UnityEngine;

public class AirState : MoveState
{
    public override void StartState(FighterController fighter)
    {
        
    }

    public override void UpdateState(FighterController fighter)
    {
        if (!fighter.Input.jump || fighter.CheckCollision(fighter.transform.position + ((fighter.Controller.radius * 3f) * Vector3.up), Vector3.down))
        {
            fighter.VerticalSpeed = Mathf.Min(0f, fighter.VerticalSpeed);
        }

        fighter.VerticalSpeed += fighter.gravity * Time.deltaTime;

        fighter.FaceDirection(fighter.Direction);
    }

    public override void ChangeState(FighterController fighter)
    {
        if (fighter.VerticalSpeed <= 0f && fighter.CheckCollision(fighter.transform.position, Vector3.up))
        {
            fighter.SetState(fighter.GroundState);
        }
    }

    public override void ExitState(FighterController fighter)
    {
        
    }
}

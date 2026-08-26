using UnityEngine;

[System.Serializable]
public class GroundState : MoveState
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;

    private bool canJump;

    public override void StartState(FighterController fighter)
    {
        fighter.VerticalSpeed = fighter.stickForce;

        canJump = false;
    }

    public override void UpdateState(FighterController fighter)
    {
        if (fighter.Input.direction != Vector3.zero)
        {
            fighter.CurrentSpeed = moveSpeed;
            fighter.Direction = fighter.Input.direction;
        }
        else
        {
            fighter.CurrentSpeed = 0f;
        }

        fighter.FaceDirection(fighter.Direction);

        if (fighter.Input.jump && canJump)
        {
            fighter.VerticalSpeed = jumpSpeed;
        }

        canJump = !fighter.Input.jump;
    }

    public override void ChangeState(FighterController fighter)
    {
        if (fighter.VerticalSpeed > 0f || !fighter.CheckCollision(fighter.transform.position, Vector3.up))
        {
            fighter.SetState(fighter.AirState);
        }
    }

    public override void ExitState(FighterController fighter)
    {
        
    }
}

using UnityEngine;

/// <summary>
/// Architecture Role: Locomotive State.
/// Manages mid-air states, applying downward gravity acceleration forces,
/// monitoring overhead ceiling constraints, and executing land-fall logic back to the ground.
/// </summary>
[System.Serializable]
public class AirState : MoveState
{
    [SerializeField] private AnimationClip jump;
    [SerializeField] private AnimationClip fall;

    public override void StartState(FighterController fighter)
    {

    }

    public override void UpdateState(FighterController fighter)
    {
        // 1. CEILING COLLISION & RELEASE DETECTOR
        // If the player lets go of the jump key mid-climb (for variable jump heights), 
        // OR if our environmental check sphere detects a solid roof directly above the character's head...
        if (!fighter.Input.jump || fighter.CheckCollision(fighter.transform.position + ((fighter.Controller.radius * 3f) * Vector3.up), Vector3.down))
        {
            // ...instantly kill all upward kinetic vertical momentum to force an immediate fall
            fighter.VerticalSpeed = Mathf.Min(0f, fighter.VerticalSpeed);
        }

        // 2. CONSTANT GRAVITATIONAL FORCE ACCELERATION
        // Continuously pull the vertical axis speed down using your design gravity variable float
        fighter.VerticalSpeed += fighter.gravity * Time.deltaTime;

        // Maintain the last horizontal face direction vector throughout the duration of the jump arc
        fighter.FaceDirection(fighter.Direction, fighter.turnSpeed);

        if (fighter.VerticalSpeed > 0.1f)
        {
            fighter.Animancer.Play(jump, 0.15f);
        }
        else if (fighter.VerticalSpeed < -0.01f)
        {
            fighter.Animancer.Play(fall, 0.15f);
        }
    }

    public override void ChangeState(FighterController fighter)
    {
        // 3. LANDFALL RECOVERY EVALUATION
        // If our vertical force has shifted fully downward (falling) AND our environmental sphere
        // check verifies a solid floor boundary layers beneath our feet, return safely to locomotion
        if (fighter.VerticalSpeed <= 0f && fighter.CheckCollision(fighter.transform.position, Vector3.up))
        {
            fighter.SetState(fighter.GroundState);
        }
    }

    public override void ExitState(FighterController fighter)
    {
        // Cleanup parameters for landing transitions sit here if adding landing recovery delays later
    }
}

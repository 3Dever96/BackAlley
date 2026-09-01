using UnityEngine;

/// <summary>
/// Architecture Role: Combat Reaction State / Flinch State.
/// Processes incoming kinetic knockback and lift forces, manages airborne gravity drops, 
/// and dynamically evaluates structural impact strength to determine whether a fighter 
/// lands cleanly or drops into a hard, downed knockdown.
/// </summary>
[System.Serializable]
public class HitState : MoveState
{
    // =========================================================================
    // HIT CONFIGURATIONS & PAYLOAD CACHES
    // =========================================================================
    [Header("Impact Threshold Balancing")]
    [SerializeField] private float recoveryThreshold; // The design baseline cutoff point where hits become heavy launches

    private AttackData attackData;                     // Local tracking container holding the raw incoming strike properties
    private bool needRecovery;                         // Internal flag tracking whether this specific hit triggers a hard knockdown

    public override void StartState(FighterController fighter)
    {
        // Verify that a valid data box was passed over via the FighterController.OnHit portal
        if (attackData != null)
        {
            // 1. PHYSICAL MOMENTUM INITIALIZATION
            // Extract the data variables from the payload and inject them directly into the hub variables
            fighter.CurrentSpeed = attackData.attack.knockbackForce;
            fighter.VerticalSpeed = attackData.attack.knockUpForce;
            fighter.Direction = attackData.direction;

            // 2. DUAL-TIER STRENGTH CHECK
            // Compare the attack's horizontal force against our design threshold limits.
            // If the incoming blast is massive, trip the flag to force a hard knockdown crash upon landing!
            needRecovery = attackData.attack.knockbackForce > recoveryThreshold;
        }
    }

    public override void UpdateState(FighterController fighter)
    {
        fighter.Animancer.Play(fighter.Anim.hit, 0.15f);

        // 3. CONSTANT GRAVITATIONAL FORCE ACCELERATION
        // Continuously pull the vertical axis speed down using your design gravity variable float
        fighter.VerticalSpeed += fighter.gravity * Time.deltaTime;

        // Maintain the last horizontal face direction vector throughout the duration of the jump arc.
        // We multiply by negative direction so the character accurately faces backwards while flying through the air!
        fighter.FaceDirection(-fighter.Direction, fighter.turnSpeed);
    }

    public override void ChangeState(FighterController fighter)
    {
        // 4. LANDFALL RECOVERY EVALUATION
        // If our vertical force has shifted fully downward (falling) AND our environmental sphere
        // check verifies a solid floor boundary layers beneath our feet, return safely to locomotion
        if (fighter.VerticalSpeed <= 0f && fighter.CheckCollision(fighter.transform.position, Vector3.up))
        {
            // 5. THE ARBITRATION ROUTER
            if (!needRecovery)
            {
                // Tier A: Flinch was low power. Return instantly back to active ground neutral movement control
                fighter.SetState(fighter.GroundState);
            }
            else
            {
                // Tier B: Heavy launch crash! Divert character into the immobile RecoveryState loop
                fighter.SetState(fighter.RecoveryState);
            }
        }
    }

    public override void ExitState(FighterController fighter)
    {
        // 6. PIPELINE HOUSEKEEPING
        // Completely wipe our local variable data container cache to prepare for the next clean hit
        attackData = null;
    }

    // =========================================================================
    // COMMUNICATIONS BRIDGE DATA ENVELOPE
    // =========================================================================
    /// <summary>
    /// Gateway method called directly by the master FighterController hub inside its OnHit execution block.
    /// Safely pipes the external physics packet right into this state's local scope memory channel.
    /// </summary>
    /// <param name="data">The self-contained AttackData package payload delivered straight from the landing blow.</param>
    public void GetAttackData(AttackData data)
    {
        attackData = data;
    }
}

using UnityEngine;

/// <summary>
/// Manages the fighter's available combat move datasets and tracks active combo chaining indices.
/// </summary>
public class AttackSystem : MonoBehaviour
{
    // ==========================================
    // SCRIPTABLE OBJECT MOVE SLOTS (DECK CONFIG)
    // ==========================================
    [Header("Ground Attack Sequence Configuration")]
    [SerializeField] private AttackMove groundComboA; // Hit 1: Opening Jab/Strike
    [SerializeField] private AttackMove groundComboB; // Hit 2: Following Link Strike
    [SerializeField] private AttackMove groundComboC; // Hit 3: Heavy Finisher Launcher

    // ==========================================
    // COMBO RUNTIME TRACKERS
    // ==========================================
    // Keeps track of how far along the player is in their multi-hit chain string
    private int currentCombo;

    /// <summary>
    /// Evaluates the context of the state the fighter just left, updates the index,
    /// and returns the correct AttackMove data cartridge to execute.
    /// </summary>
    /// <param name="currentState">The MoveState context being used for conditional filtering (e.g., fighter.LastState).</param>
    /// <returns>The data payload for the next attack step, or null if filtering checks fail or the string ends.</returns>
    public AttackMove GetCurrentAttack(MoveState currentState)
    {
        // 1. CONTEXT CHECK: Determine if the attack was initiated from neutral floor locomotion
        if (currentState.GetType() == typeof(GroundState))
        {
            // 2. ARRAY ASSEMBLY: Pack our individual attack card slots into a temporary evaluation array
            AttackMove[] groundCombo = new AttackMove[3];
            groundCombo[0] = groundComboA;
            groundCombo[1] = groundComboB;
            groundCombo[2] = groundComboC;

            // 3. STEP BOUNDARY LOOKUP: Verify we haven't mashed past the final slot in our sequence array
            if (currentCombo < groundCombo.Length)
            {
                // Advance our tracker safely to prime the next hit in the chain for the next cycle
                currentCombo += 1;

                // Return the historical slot matching the current hit (Array is 0-indexed, so we subtract 1)
                return groundCombo[currentCombo - 1];
            }
        }

        // Return fallback null safety value if they try to attack from an unsupported state profile
        return null;
    }

    /// <summary>
    /// Completely wipes the active execution string index, resetting the combo deck back to Hit 1.
    /// </summary>
    public void ResetCombo()
    {
        currentCombo = 0;
    }
}

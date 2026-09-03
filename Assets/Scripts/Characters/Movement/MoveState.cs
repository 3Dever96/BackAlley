using UnityEngine;

/// <summary>
/// Architecture Role: Abstract Base State (State Pattern Blueprint).
/// Serves as the master template for all locomotive and combat phases (GroundState, AirState, AttackState).
/// Every state script must inherit from this class and implement these four core lifecycle loops,
/// allowing the central FighterController hub to swap and execute them seamlessly at runtime.
/// </summary>
[System.Serializable]
public abstract class MoveState
{
    /// <summary>
    /// Lifecycle Step 1: Execution Initialization.
    /// Triggered precisely on the single frame the state machine transitions into this state.
    /// Use this to initialize variables, turn on colliders, alter speed caps, or fire off animation clips.
    /// </summary>
    /// <param name="fighter">The central master FighterController hub invoking this state.</param>
    public abstract void StartState(FighterController fighter);

    /// <summary>
    /// Lifecycle Step 2: Continuous Frame Execution.
    /// Triggered every single frame during MonoBehaviour.Update() while this state remains active.
    /// Use this to accumulate timers, read thumbstick vectors, apply gravity calculations, or adjust orientation rotations.
    /// </summary>
    /// <param name="fighter">The central master FighterController hub invoking this state.</param>
    public abstract void UpdateState(FighterController fighter);

    /// <summary>
    /// Lifecycle Step 3: Transition Monitoring.
    /// Triggered immediately after UpdateState() inside the main game loop.
    /// Use this to continuously evaluate if condition boundaries have been met to break out of this state
    /// and jump into another context (e.g., checking if an attack timer has concluded to switch back to ground control).
    /// </summary>
    /// <param name="fighter">The central master FighterController hub invoking this state.</param>
    public abstract void ChangeState(FighterController fighter);

    /// <summary>
    /// Lifecycle Step 4: Cleanup & Exit Rules.
    /// Triggered precisely on the single frame the state machine leaves this state to transition into a new one.
    /// Use this to safely tear down variable toggles, shut off hitboxes, clear input queues, or reset counters.
    /// </summary>
    /// <param name="fighter">The central master FighterController hub invoking this state.</param>
    public abstract void ExitState(FighterController fighter);
}

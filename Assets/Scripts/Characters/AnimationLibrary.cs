using UnityEngine;

/// <summary>
/// Architecture Role: Central Animation Asset Registry (Spoke / Data Warehouse Component).
/// Acts as a unified, decoupled repository hosting all core skeletal locomotion 
/// and combat clip assets required by a specific character archetype.
/// By storing these references on a centralized component adjacent to the main hub, 
/// independent state modules (like GroundState, AirState, or HitState) can quickly fetch 
/// their required clip handles on demand without needing to hardcode unique asset lookups
/// directly inside the decoupled state scripts.
/// </summary>
public class AnimationLibrary : MonoBehaviour
{
    // ==========================================
    // NEUTRAL LOCOMOTION CLIPS
    // ==========================================
    [Header("Locomotion Animations")]
    // Played continuously inside GroundState when horizontal input vectors collapse to neutral zero.
    public AnimationClip idle;

    // Played continuously inside GroundState when horizontal joystick input direction is active.
    public AnimationClip run;

    // Triggered the exact frame GroundState shifts to AirState via an upward vertical impulse force.
    public AnimationClip jump;

    // Triggered continuously inside AirState when vertical speed metrics cross into negative thresholds.
    public AnimationClip fall;

    // ==========================================
    // COMBAT REACTION CLIPS
    // ==========================================
    [Header("Combat Reaction Animations")]
    // Triggered on entry inside HitState to display a directional flinch reaction from lightweight attacks.
    public AnimationClip hit;

    // Triggered on entry inside HitState when an incoming knockback force breaches the launch threshold bounds.
    public AnimationClip knockout;

    // Triggered on entry inside RecoveryState when a downed character executes their get-up timeline sequence.
    public AnimationClip recover;

    // ==========================================
    // ENDGAME EVALUATION CLIPS
    // ==========================================
    [Header("Match Conclusion Animations")]
    // Triggered on override by the BattleManager referee engine when global victory state conditions are met.
    public AnimationClip victory;
}

using UnityEngine;

/// <summary>
/// Architecture Role: Abstract Input Contract (Base Class / Interface Spoke).
/// Acts as a universal, clean data container bridge between raw input drivers 
/// (PlayerController or AIController) and the master central FighterController hub.
/// By reading inputs from this abstract base, your movement and attack states do not care 
/// whether the fighter is being driven by a human or a computer mind.
/// </summary>
public class InputController : MonoBehaviour
{
    // ==========================================
    // ANCHOR LOCOMOTION VECTOR
    // ==========================================
    [Header("Movement Input Vector")]
    // The raw directional vector (typically mapped from a thumbstick or WASD keys).
    // The state machine reads this to calculate facing directions and running velocities.
    public Vector3 direction;

    // ==========================================
    // STANDARD ARCADE ACTION STATES
    // ==========================================
    [Header("Core Standard Combat Buttons")]
    // True on the frame the player presses the jump key (or when the AI hits an obstacle raycast).
    public bool jump;

    // True on the frame the user taps attack. Feeds straight into your AttackState inputBuffer loop.
    public bool attack;

    // True while the block key is actively held down. Forces the fighter into a defensive stance.
    public bool block;

    // ==========================================
    // RETRO HIGH-VELOCITY UTILITIES
    // ==========================================
    [Header("Advanced Locomotion Utilities")]
    // Tracks burst inputs to trigger high-velocity horizontal air and ground escapes.
    public bool dash;

    // Camera/Target tracking utility lock toggle flag (True to fix camera view constraints).
    public bool lockOn;

    // ==========================================
    // DATA-DRIVEN SPECIAL SKILL EQUIPS
    // ==========================================
    [Header("Modular Move Customizations")]
    // Activates your specialized Custom Move Card assigned to the Alpha special slot.
    public bool special1;

    // Activates your specialized Custom Move Card assigned to the Beta special slot.
    public bool special2;
}

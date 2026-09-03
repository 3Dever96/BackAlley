using UnityEngine;
using UnityEngine.InputSystem; // Required directory for Unity's New Input System Package

/// <summary>
/// Architecture Role: Spoke Component / Human Input Driver.
/// Intercepts physical controller callbacks via Unity's PlayerInput engine,
/// translates thumbstick directions relative to the scene's main camera viewport,
/// and stores the raw binary data inside the abstract InputController baseline properties.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : InputController
{
    // =========================================================================
    // NEW INPUT SYSTEM INTERNAL REFERENCES
    // =========================================================================
    private PlayerInput input;          // Local reference cache to the core PlayerInput processor component
    private Vector2 move;               // Temporary storage container holding raw horizontal/vertical stick axis floats

    private void Awake()
    {
        // Cache our PlayerInput controller listener immediately at boot
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        // 1. CAMERA-RELATIVE DIRECTIONAL TRACKING
        // Fetch the current camera viewport vectors so pushing "Up" on the stick 
        // always forces the character to run deep into the screen away from the player camera angle.
        Transform cam = Camera.main.transform;

        // Project stick vector inputs onto the horizontal camera alignment plane axis
        direction = cam.right * move.x + cam.forward * move.y;

        // Zero out the vertical Y height variable so camera tilt angles don't pull the player into the sky or floor
        direction.y = 0f;

        // Normalize the vector direction to ensure diagonal running speeds don't accidentally double movement velocity scalars
        direction = direction.normalized;
    }

    private void OnEnable()
    {
        // EVENT SUBSCRIPTION: Hook our custom input evaluator directly to Unity's global device listener broadcast channel
        input.onActionTriggered += OnAction;
    }

    private void OnDisable()
    {
        // SAFETY UNHOOK: Forcefully sever the event link when the controller script turns off to protect system memory
        input.onActionTriggered -= OnAction;

        input.enabled = false;
    }

    /// <summary>
    /// Master Action Callback Router. Intercepts named device input maps, extracts active data types,
    /// and flags the abstract base class boolean properties exactly when keys are pressed or released.
    /// </summary>
    /// <param name="context">The active input payload data bundle sent down by the hardware device processor.</param>
    public void OnAction(InputAction.CallbackContext context)
    {
        // Switch evaluation route based on the explicit string name defined inside your Unity Input Actions Asset Map
        switch (context.action.name)
        {
            case "Move":
                // Extract the active continuous 2D Vector coordinate position data box (-1 to 1 bounds)
                move = context.ReadValue<Vector2>();
                break;

            case "Jump":
                // Read button engagement depth. A float scalar above 0.5 translates to a true button press flag
                jump = context.ReadValue<float>() > 0.5f;
                break;

            case "Attack":
                // Feeds directly into your AttackState update engine inputBuffer checks!
                attack = context.ReadValue<float>() > 0.5f;
                break;

            case "Block":
                block = context.ReadValue<float>() > 0.5f;
                break;

            case "Dash":
                dash = context.ReadValue<float>() > 0.5f;
                break;

            case "LockOn":
                lockOn = context.ReadValue<float>() > 0.5f;
                break;

            case "Special1":
                special1 = context.ReadValue<float>() > 0.5f;
                break;

            case "Special2":
                special2 = context.ReadValue<float>() > 0.5f;
                break;
        }
    }
}

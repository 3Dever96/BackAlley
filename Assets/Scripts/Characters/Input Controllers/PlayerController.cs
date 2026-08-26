using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : InputController
{
    private PlayerInput input;

    private Vector2 move;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Transform cam = Camera.main.transform;

        direction = cam.right * move.x + cam.forward * move.y;
        direction.y = 0f;
        direction = direction.normalized;
    }

    private void OnEnable()
    {
        input.onActionTriggered += OnAction;
    }

    private void OnDisable()
    {
        input.onActionTriggered -= OnAction;
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Move": move = context.ReadValue<Vector2>(); break;
            case "Jump": jump = context.ReadValue<float>() > 0.5f; break;
            case "Attack": attack = context.ReadValue<float>() > 0.5f; break;
            case "Block": block = context.ReadValue<float>() > 0.5f; break;
            case "Dash": dash = context.ReadValue<float>() > 0.5f; break;
            case "LockOn": lockOn = context.ReadValue<float>() > 0.5f; break;
            case "Special1": special1 = context.ReadValue<float>() > 0.5f; break;
            case "Special2": special2 = context.ReadValue<float>() > 0.5f; break;
        }
    }
}

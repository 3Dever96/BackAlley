using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FighterController : MonoBehaviour
{
    // References
    public CharacterController Controller {  get; private set; }
    public InputController Input { get; private set; }
    public HealthSystem Health { get; private set; }
    public AttackSystem Attack { get; private set; }
    public HurtboxController Hurtbox { get; private set; }

    // State Machine
    public MoveState CurrentState { get; private set; }
    public MoveState LastState {  get; private set; }
    [field: SerializeField] public GroundState GroundState { get; private set; } = new GroundState();
    [field: SerializeField] public AirState AirState { get; private set; } = new AirState();
    [field: SerializeField] public AttackState AttackState { get; private set; } = new AttackState();

    // Variables
    public Vector3 Direction { get; set; }
    public Vector3 Velocity { get; set; }
    public float CurrentSpeed { get; set; }
    public float VerticalSpeed { get; set; }

    [Header("Shared Variables")]
    public float turnSpeed;
    public float stickForce;
    public float gravity;

    // Input
    public InputType inputType;

    private PlayerController player;
    private AIController ai;

    private void Start()
    {
        Controller = GetComponent<CharacterController>();
        Health = GetComponent<HealthSystem>();
        Attack = GetComponent<AttackSystem>();

        if (inputType == InputType.AI)
        {
            Health.SetBot(true);
        }

        Hurtbox = GetComponentInChildren<HurtboxController>();

        SetInput();

        SetState(GroundState);
    }

    private void Update()
    {
        if (BattleManager.instance.state == BattleState.Fight)
        {
            if (CurrentState != null)
            {
                CurrentState.UpdateState(this);
                CurrentState.ChangeState(this);

                ApplyMovement();
            }
        }
    }

    public void SetState(MoveState newState)
    {
        if (CurrentState != null)
        {
            CurrentState.ExitState(this);

            if (CurrentState != newState)
            {
                LastState = CurrentState;
            }
        }

        CurrentState = newState;

        if (CurrentState != null)
        {
            CurrentState.StartState(this);
        }
    }

    public void FaceDirection(Vector3 lookDirection, float turnSpeed = 500f)
    {
        if (lookDirection == Vector3.zero) return;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDirection), turnSpeed * Time.deltaTime);
    }

    public void ApplyMovement()
    {
        Vector3 velocity = CurrentSpeed * Direction;
        velocity.y = VerticalSpeed;

        Velocity = velocity;

        Controller.Move(Velocity * Time.deltaTime);
    }

    public bool CheckCollision(Vector3 origin, Vector3 offset)
    {
        float radius = Controller.radius;

        return Physics.CheckSphere(origin + ((radius - 0.2f) * offset), radius - 0.1f, LayerMask.GetMask("Solid"));
    }

    private void SetInput()
    {
        player = GetComponent<PlayerController>();
        ai = GetComponent<AIController>();

        if (inputType == InputType.Player)
        {
            player.enabled = true;
            ai.enabled = false;

            Input = player;

            CinemachineCamera cm = FindAnyObjectByType<CinemachineCamera>();
            CameraTarget target = new CameraTarget();

            target.LookAtTarget = transform;
            target.TrackingTarget = transform;

            cm.Target = target;
        }
        else
        {
            ai.enabled = true;
            player.enabled = false;

            Input = ai;
        }
    }
}

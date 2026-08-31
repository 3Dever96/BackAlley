using Unity.Cinemachine; // Required for Cinemachine camera targeting hooks
using UnityEngine;
using Animancer;

/// <summary>
/// Architecture Role: Central Information Hub & State Machine Master.
/// Caches sub-system spoke scripts, runs continuous frame updates, and drives 
/// kinematic CharacterController translation loops based on the active state.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class FighterController : MonoBehaviour
{
    // ==========================================
    // CACHED HUB SPOKE REFERENCES
    // ==========================================
    public CharacterController Controller { get; private set; }
    public InputController Input { get; private set; }
    public HealthSystem Health { get; private set; }
    public AttackSystem Attack { get; private set; }
    public HurtboxController Hurtbox { get; private set; }
    public AnimancerComponent Animancer { get; private set; }

    // ==========================================
    // STATE MACHINE PATTERN SETUPS
    // ==========================================
    public MoveState CurrentState { get; private set; }
    public MoveState LastState { get; private set; }

    [field: SerializeField] public GroundState GroundState { get; private set; } = new GroundState();
    [field: SerializeField] public AirState AirState { get; private set; } = new AirState();
    [field: SerializeField] public AttackState AttackState { get; private set; } = new AttackState();
    [field: SerializeField] public HitState HitState { get; private set; } = new HitState();
    [field: SerializeField] public RecoveryState RecoveryState { get; private set; } = new RecoveryState();

    // ==========================================
    // KINEMATIC LOCOMOTION METRICS
    // ==========================================
    public Vector3 Direction { get; set; }      // Active horizontal heading vector feeding ApplyMovement
    public Vector3 Velocity { get; set; }       // Final combined velocity vector pushed into Controller.Move
    public float CurrentSpeed { get; set; }     // Horizontal run velocity speed scalar
    public float VerticalSpeed { get; set; }    // Vertical physics forces (gravity pulls or jump velocities)

    [Header("Shared Locomotion Properties")]
    public float turnSpeed;                     // Angular rotation snap tracking speed
    public float stickForce;                    // Controller deadzone threshold values
    public float gravity;                       // Constant downward force speed acceleration

    [Header("Input Driver Configuration")]
    public InputType inputType;                 // State toggle setting Player vs AI brain routing
    private PlayerController player;
    private AIController ai;

    private void Start()
    {
        // Cache all local operational components immediately at boot execution
        Controller = GetComponent<CharacterController>();
        Health = GetComponent<HealthSystem>();
        Attack = GetComponent<AttackSystem>();
        Animancer = GetComponentInChildren<AnimancerComponent>();

        // If marked as an AI adversary, route through the specialized health logger tracking rules
        if (inputType == InputType.AI)
        {
            Health.SetBot(true);
        }

        Hurtbox = GetComponentInChildren<HurtboxController>();

        // Configure input driver components
        SetInput();

        // Drop the fighter into neutral ground locomotion to begin the match loop
        SetState(GroundState);
    }

    private void Update()
    {
        // Global Match Gate: Frame execution loops only execute if the match referee says "Fight"
        if (BattleManager.instance.state == BattleState.Fight)
        {
            if (CurrentState != null)
            {
                // Run continuous state evaluation logic and transition monitoring
                CurrentState.UpdateState(this);
                CurrentState.ChangeState(this);

                // Translate structural properties down into physical movement changes
                ApplyMovement();
            }
        }
    }

    /// <summary>
    /// Swaps the active gear of our character state machine, running entry and exit rules.
    /// Includes historical tracking overrides to protect loop pipelines.
    /// </summary>
    public void SetState(MoveState newState)
    {
        if (CurrentState != null)
        {
            // Execute the cleanup logic bound to the state we are breaking out of
            CurrentState.ExitState(this);

            // ANTI-RECURSION SAFEGUARD: Only assign LastState if we are transitioning 
            // into a genuinely new class context. If an attack re-enters itself mid-combo,
            // LastState stays locked onto GroundState so your data index lookups never break!
            if (CurrentState != newState)
            {
                LastState = CurrentState;
            }
        }

        // Assign and jump into the entry rules of the incoming state frame
        CurrentState = newState;

        if (CurrentState != null)
        {
            CurrentState.StartState(this);
        }
    }

    /// <summary>
    /// Rotates the 3D model transform matrix toward an explicit orientation vector over time.
    /// </summary>
    public void FaceDirection(Vector3 lookDirection, float turnSpeed = 500f)
    {
        if (lookDirection == Vector3.zero) return;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(lookDirection),
            turnSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// Blends active horizontal direction metrics with vertical velocity vectors to slide the character box.
    /// </summary>
    public void ApplyMovement()
    {
        Vector3 velocity = CurrentSpeed * Direction;
        velocity.y = VerticalSpeed;

        Velocity = velocity;

        // Perform physical translation push inside the Unity simulation world
        Controller.Move(Velocity * Time.deltaTime);
    }

    /// <summary>
    /// Performance optimized environmental boundary check casting a flat capsule sphere projection.
    /// </summary>
    public bool CheckCollision(Vector3 origin, Vector3 offset)
    {
        float radius = Controller.radius;
        return Physics.CheckSphere(origin + ((radius - 0.2f) * offset), radius - 0.1f, LayerMask.GetMask("Solid"));
    }

    /// <summary>
    /// Handles script activation boundaries and hooks Cinemachine camera logic to human users.
    /// </summary>
    private void SetInput()
    {
        player = GetComponent<PlayerController>();
        ai = GetComponent<AIController>();

        if (inputType == InputType.Player)
        {
            player.enabled = true;
            ai.enabled = false;
            Input = player; // Map general base reference slot to player script data

            // Target the Cinemachine framing rig onto this user instance automatically
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
            Input = ai; // Map general base reference slot to AI brain logic
        }
    }

    // =========================================================================
    // DYNAMIC REACTION MESSAGE RECEIVER
    // =========================================================================
    /// <summary>
    /// Intercepts the "OnHit" message broadcasted upward by the adjacent HealthSystem.
    /// Acts as the central pipeline transition bridge forcing the state machine 
    /// directly out of whatever it was doing straight into the combat flinch cycle.
    /// </summary>
    /// <param name="data">The self-contained AttackData package payload delivered straight from the landing blow.</param>
    public void OnHit(AttackData data)
    {
        // 1. DATA INITIALIZATION TRANSFER: Pass the complete payload packet directly into our HitState cache method
        // This ensures the HitState can immediately read the attack's forces and vector headings on entry
        HitState.GetAttackData(data);

        // 2. FORCED TRANSITION: Instantly break out of locomotion/attacking and shift gears into the hit reaction
        SetState(HitState);
    }
}

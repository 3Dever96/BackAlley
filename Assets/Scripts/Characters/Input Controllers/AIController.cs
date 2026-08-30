using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Architecture Role: Spoke Component / Input Brain.
/// Evaluates the combat arena layout, dynamically selects the closest active target,
/// and feeds 2D locomotion direction vectors back into the master FighterController central hub.
/// </summary>
public class AIController : InputController
{
    // =========================================================================
    // TARGET ACQUISITION & EVALUATION VARIABLES
    // =========================================================================
    private Transform currentTarget;                             // The transform reference of the actively hunted opponent
    private List<FighterController> possibleTargets = new List<FighterController>(); // Master tracker cache of all foreign fighters in the match
    private FighterController myController;                     // Reference to this entity's own central hub controller

    [Header("Performance Optimization")]
    private float targetEvaluationTimer = 0f;                   // Current running accumulator clock for interval scans
    private float targetEvaluationInterval = 0.2f;              // Throttle gate: restricts heavy distance sorting math to 5 times a second

    [Header("Combat Tracking Thresholds")]
    [SerializeField] private float attackRange;                  // Distance cushion where the AI drops its heading vector to strike

    private void Start()
    {
        // Cache our own central hub system on startup
        myController = GetComponent<FighterController>();

        // 1. SCENE DISCOVERY LOOP: Gather every active fighter entity currently deployed in the scene structure
        FighterController[] fighters = Object.FindObjectsByType<FighterController>();
        for (var i = 0; i < fighters.Length; i++)
        {
            // Filter Layer: Ensure the AI doesn't accidentally register its own self as a valid target array option
            if (fighters[i] != myController)
            {
                possibleTargets.Add(fighters[i]);
            }
        }

        // Run an immediate initialization target scan so the AI doesn't stand still on frame 1
        EvaluateNextTarget();
    }

    private void Update()
    {
        // 2. TIMED PERFORMANCE GATE: Clock down our environment re-evaluation interval
        targetEvaluationTimer += Time.deltaTime;
        if (targetEvaluationTimer >= targetEvaluationInterval)
        {
            targetEvaluationTimer = 0f;
            EvaluateNextTarget(); // Look to see if an un-tracked opponent wandered closer
        }

        // 3. LOCOMOTION VECTOR MANIPULATION BLOCK
        if (currentTarget != null)
        {
            // FLAT PLANE CONVERSION: Project 3D positions down onto a flat 2D plane (X and Z)
            // This stops vertical jump vectors or air juggles from completely breaking the AI's ground chase distance calculations
            Vector2 myPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 targetPosition = new Vector2(currentTarget.position.x, currentTarget.position.z);

            // 4. COMBAT SPACING THRESHOLD EVALUATION
            if (Vector2.Distance(myPosition, targetPosition) > attackRange)
            {
                // CASE A: Target is too far away. Calculate direct tracking heading vector
                direction = currentTarget.position - transform.position;

                // Zero out vertical Y height axis differences so the AI doesn't try to tilt its movement into the dirt
                direction.y = 0f;

                // Normalize the heading to ensure the vector strictly transmits safe, uniform movement speeds
                direction = direction.normalized;
            }
            else
            {
                // CASE B: Inside striking bounds! Collapse movement vectors to a dead stop to prevent collider-climbing glitches
                direction = Vector3.zero;

                // (Tomorrow: This is the exact condition block where you will trigger your AttackState switches!)
            }
        }
    }

    /// <summary>
    /// Performance-optimized scanning block. Evaluates the physical proximity matrix of all known fighters 
    /// and snaps currentTarget focus directly onto the closest surviving candidate.
    /// </summary>
    private void EvaluateNextTarget()
    {
        FighterController bestTarget = null;
        float closestDistance = float.MaxValue; // Initialize sorting ceiling value to infinity

        // Iterate backwards through the array index to ensure safe removals if objects get destroyed mid-match
        for (int i = possibleTargets.Count - 1; i >= 0; i--)
        {
            FighterController targetFighter = possibleTargets[i];

            // 5. LIFECYCLE SCRUBBING CLEANUP SAFETY DOORS
            // If a targeted entity has been cleanly deleted or deleted by scene updates, skip calculation loops completely
            if (targetFighter == null/* || targetFighter.CurrentState == targetFighter.KnockoutState*/)
            {
                continue;
            }

            // 6. PROXIMITY DISTANCE COMPARISON
            float distance = Vector3.Distance(transform.position, targetFighter.transform.position);
            if (distance < closestDistance)
            {
                // New winner found! Update tracking bounds and lock reference down
                closestDistance = distance;
                bestTarget = targetFighter;
            }
        }

        // 7. ARBITRATION ASSIGNMENT
        if (bestTarget != null)
        {
            currentTarget = bestTarget.transform;
        }
        else
        {
            // Null Void Fallback: No surviving fighters left alive in the entire arena profile container
            currentTarget = null;
        }
    }

    /// <summary>
    /// Public Event Interface. Triggered downstream by your FighterStats/HealthSystem arrays when a hit registers.
    /// Simulates the classic Sonic Battle high-aggro target swap response.
    /// </summary>
    /// <param name="attacker">The parent GameObject entity that owned the Hurtbox trigger causing damage.</param>
    public void OnTakeDamageFrom(GameObject attacker)
    {
        FighterController attackerController = attacker.GetComponent<FighterController>();

        // 8. THE REVENGE SWITCH ENVELOPE
        if (attackerController != null && possibleTargets.Contains(attackerController))
        {
            // Instantly override our tracking heading to lock directly onto our attacker for immediate counterpunches
            currentTarget = attacker.transform;

            // Shift timer value deep into negative thresholds. This forces the periodic distance update matrix 
            // to briefly stall out for over a second, ensuring the AI sticks to its revenge focus instead of instantly snapping back
            targetEvaluationTimer = -1f;
        }
    }
}

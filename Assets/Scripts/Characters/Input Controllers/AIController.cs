using System.Collections.Generic;
using UnityEngine;

public class AIController : InputController
{
    private Transform currentTarget;
    private List<FighterController> possibleTargets = new List<FighterController>();

    private FighterController myController;
    private float targetEvaluationTimer = 0f;
    private float targetEvaluationInterval = 0.2f; // Check environment 5 times a second

    [SerializeField] private float attackRange;

    private void Start()
    {
        myController = GetComponent<FighterController>();

        // Store references to the actual controllers so we can check their active health/states later
        FighterController[] fighters = Object.FindObjectsByType<FighterController>();
        for (var i = 0; i < fighters.Length; i++)
        {
            if (fighters[i] != myController)
            {
                possibleTargets.Add(fighters[i]);
            }
        }

        EvaluateNextTarget();
    }

    private void Update()
    {
        // Periodic check to see if someone else walked closer
        targetEvaluationTimer += Time.deltaTime;
        if (targetEvaluationTimer >= targetEvaluationInterval)
        {
            targetEvaluationTimer = 0f;
            EvaluateNextTarget();
        }

        // Proceed with your movement/spacing logic using currentTarget...

        if (currentTarget != null)
        {
            Vector2 myPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 targetPosition = new Vector2(currentTarget.position.x, currentTarget.position.z);

            if (Vector2.Distance(myPosition, targetPosition) > attackRange)
            {
                direction = currentTarget.position - transform.position;
                direction.y = 0f;
                direction = direction.normalized;
            }
            else
            {
                direction = Vector3.zero;
            }
        }
    }

    private void EvaluateNextTarget()
    {
        FighterController bestTarget = null;
        float closestDistance = float.MaxValue;

        for (int i = possibleTargets.Count - 1; i >= 0; i--)
        {
            FighterController targetFighter = possibleTargets[i];

            // 1. Clean up: If a target was deleted or is Knocked Out, ignore them
            if (targetFighter == null/* || targetFighter.CurrentState == targetFighter.KnockoutState*/)
            {
                continue;
            }

            // 2. Proximity calculation
            float distance = Vector3.Distance(transform.position, targetFighter.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = targetFighter;
            }
        }

        // 3. Assign the winning target
        if (bestTarget != null)
        {
            currentTarget = bestTarget.transform;
        }
        else
        {
            currentTarget = null; // No one left alive! Victory state handles this elsewhere
        }
    }

    // PUBLIC INTERFACE: Call this from your FighterStats/Health system when this AI takes damage!
    public void OnTakeDamageFrom(GameObject attacker)
    {
        FighterController attackerController = attacker.GetComponent<FighterController>();

        if (attackerController != null && possibleTargets.Contains(attackerController))
        {
            // Sonic Battle Revenge Switch: Instantly swap target to whoever just hit you
            currentTarget = attacker.transform;
            targetEvaluationTimer = -1f; // Briefly stall periodic evaluation so it doesn't instantly snap back
        }
    }
}

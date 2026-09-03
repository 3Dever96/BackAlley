using UnityEngine;

/// <summary>
/// Controls the active execution, dimensions, and trigger collision events of the fighter's front-facing strike volume.
/// </summary>
public class HurtboxController : MonoBehaviour
{
    // ==========================================
    // PHYSICAL GEOMETRY PROPERTIES
    // ==========================================
    [Header("Collider Child Setup")]
    [SerializeField] private GameObject hurtbox; // The physical child volume housing the Trigger Collider mesh

    // ==========================================
    // LOCAL PACKAGED MOVE ATTRIBUTES
    // ==========================================
    private AttackMove currentMove;              // Cached local storage reference for the active action's parameters

    /// <summary>
    /// Resizes the physical dimensions of the front striking container directly from ScriptableObject asset data
    /// and opens the trigger boundaries to make the attack active and dangerous.
    /// </summary>
    /// <param name="newMove">The specialized AttackMove cartridge passed down by the master central AttackState.</param>
    public void ActivateHitBox(AttackMove newMove)
    {
        // 1. SAFETY FLIP: Forcefully shut off the child object frame buffer before changing dimensions.
        // This stops Unity's physics engine from accidentally sweeping through space and triggering phantom hits!
        hurtbox.SetActive(false);

        // Cache our incoming move statistics
        currentMove = newMove;

        // 2. SCALE TRANSLATION: Stretch our physical transform scale coordinates to match the move's custom width/height/depth
        transform.localScale = currentMove.hurtboxSize;

        // 3. ENGAGE BOX: Re-activate the collider object to make it dangerous to surrounding entities
        hurtbox.SetActive(true);
    }

    /// <summary>
    /// Deactivates the frontal striking volume, cleans up active variables, and resets the transform matrices
    /// back to neutral parameters so it doesn't warp adjacent character meshes.
    /// </summary>
    public void DeactivateHitBox()
    {
        // Collapse active trigger physical properties instantly
        hurtbox.SetActive(false);

        // Return local scaling structures back to a perfect, unified cube format
        transform.localScale = Vector3.one;

        // Clean out data references to protect storage caches
        currentMove = null;
    }

    /// <summary>
    /// Unity Engine Physics Trigger Callback loop. Intercepts intersecting rigid body capsule envelopes,
    /// searches for valid health managers, and transmits raw damage statistics down into target entities.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Search upward through the overlapping object's hierarchy chain to find its main health script spoke
        HealthSystem health = other.GetComponentInParent<HealthSystem>();

        // =========================================================================
        // 4. DAMAGE & PHYSICS PAYLOAD DELIVERY
        // =========================================================================
        if (health != null)
        {
            // PACKAGING THE STRIKE: Instantiate our new AttackData container.
            // We pass the active ScriptableObject move stats alongside 'transform.forward' 
            // so the victim's hit state knows exactly which directional heading to fly backward in!
            AttackData attackData = new AttackData(currentMove, transform.forward);

            // Direct Call: Hand off the complete self-contained data package straight into the opponent's health engine
            health.TakeDamage(attackData);
        }
    }
}

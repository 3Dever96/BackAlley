using UnityEngine;

/// <summary>
/// Architecture Role: Combat Payload Container (Pure Data Class).
/// Acts as a standalone, decoupled data ferry that wraps up combat statistics 
/// alongside dynamic spatial vectors at the exact millisecond a strike connects.
/// By passing this self-contained package down into a victim's health and state systems,
/// the receiving character knows exactly how much damage to take and which direction 
/// to fly without needing a direct, two-way connection back to the attacker.
/// </summary>
[System.Serializable]
public class AttackData
{
    // =========================================================================
    // PACKAGED COMBAT PARAMETERS
    // =========================================================================
    // The specific ScriptableObject move cartridge containing damage, strength, and force stats
    public AttackMove attack;

    // The physical 3D directional vector pointing from the center of the attacker to the victim
    public Vector3 direction;

    /// <summary>
    /// Data Constructor. Seals the structural move properties and directional vectors 
    /// together into a unified immutable package at the frame of collision.
    /// </summary>
    /// <param name="_attack">The active AttackMove scriptable data asset loaded on the swinging fist.</param>
    /// <param name="_direction">The calculated directional heading pointing straight toward the target entity.</param>
    public AttackData(AttackMove _attack, Vector3 _direction)
    {
        attack = _attack;
        direction = _direction;
    }
}

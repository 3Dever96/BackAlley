using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack")]
public class AttackMove : ScriptableObject
{
    [Header("Identity & Info")]
    public string attackName;
    [TextArea] public string attackDescription;

    [Header("Animation Assets")]
    // The raw clip asset that your central hub will hand over to Animancer
    public AnimationClip attackAnimation;
    public float attackDuration; // Total execution/recovery time spent in the state

    [Header("Combo Chain Timing")]
    // The precise timestamp during the attack where pressing the button 
    // successfully triggers the next move in the combo chain
    public float inputBuffer;

    [Header("Combat Payload Statistics")]
    public float attackPow;      // Damage integer value
    public float knockUpForce;   // Vertical lift vector modifier
    public float knockbackForce; // Horizontal push vector modifier

    [Header("Hurtbox Settings (Deals Damage)")]
    // The exact Vector3 dimensions your single master child box 
    // will scale to whenever this unique move is executed
    public Vector3 hurtboxSize;
}

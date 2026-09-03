using UnityEngine;
using UnityEngine.UI; // Required directory for Unity's standard UI Component interactions

/// <summary>
/// Architecture Role: Spoke Component / User Interface Driver.
/// Locomotion-independent interface hub. Scans the active scene at initialization, 
/// filters out AI bots to isolate the human user, and links the UI slider values 
/// directly onto the player's event-driven health broadcast loop.
/// </summary>
public class PlayerHealthbar : MonoBehaviour
{
    // =========================================================================
    // PHYSICAL UI CANVAS COMPONENTS
    // =========================================================================
    [Header("UI Slider Anchor")]
    public Slider healthbar; // The slider component visual fill bar representing health capacity

    private void Start()
    {
        // 1. SCENE REGISTRATION DISCOVERY
        // Query the loaded scene to discover all deployed fighter entities currently in play
        FighterController[] fighters = FindObjectsByType<FighterController>();

        for (var i = 0; i < fighters.Length; i++)
        {
            // 2. HUMAN IDENTIFICATION FILTER
            // Isolate the human user by validating their custom inputType profile enum setting
            if (fighters[i].inputType == InputType.Player)
            {
                // Extract the corresponding health system component attached adjacent to the hub
                HealthSystem hp = fighters[i].GetComponent<HealthSystem>();

                // 3. EVENT-DRIVEN SUBSCRIPTION HOOK
                // Unidirectional Link: Hook our local UpdateHealthBar function directly onto the 
                // player's local OnHealthChanged event channel. 
                // This UI bar now sits completely silent, using 0% CPU power until the player takes damage!
                hp.OnHealthChanged += UpdateHealthBar;
            }
        }
    }

    /// <summary>
    /// Event Delegate Callback. Instructs the UI slider geometry to adjust its visual fill
    /// the exact split-second the player's current health integer value fluctuates.
    /// </summary>
    /// <param name="newValue">The newly rounded currentHealth integer passed up by the HealthSystem event wrapper.</param>
    public void UpdateHealthBar(int newValue)
    {
        // Direct Assignment: Snap the visual layout value to match the incoming data metric
        healthbar.value = newValue;
    }
}

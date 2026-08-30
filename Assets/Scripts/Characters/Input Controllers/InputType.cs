/// <summary>
/// Architecture Role: Structural Configuration Identity Switch.
/// Defines the available input driver profiles for a character entity.
/// The central FighterController hub reads this flag at startup to determine 
/// whether to activate human player camera tracking or computer-controlled brain sub-systems.
/// </summary>
public enum InputType
{
    // Mapped when a human user is driving the entity. 
    // Enables PlayerController scripts and automatically binds the local Cinemachine tracking camera.
    Player,

    // Mapped when a computer-controlled script is driving the entity.
    // Enables AIController scripts, registers the bot to global referee counts, and unlocks tracking arrays.
    AI
}

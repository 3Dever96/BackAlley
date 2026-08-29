using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    [SerializeField] private AttackMove groundComboA;
    [SerializeField] private AttackMove groundComboB;
    [SerializeField] private AttackMove groundComboC;

    private int currentCombo;

    public AttackMove GetCurrentAttack(MoveState currentState)
    {
        if (currentState.GetType() == typeof(GroundState))
        {
            AttackMove[] groundCombo = new AttackMove[3];

            groundCombo[0] = groundComboA;
            groundCombo[1] = groundComboB;
            groundCombo[2] = groundComboC;

            if (currentCombo < groundCombo.Length)
            {
                currentCombo += 1;

                return groundCombo[currentCombo - 1];
            }
        }

        return null;
    }

    public void ResetCombo()
    {
        currentCombo = 0;
    }
}

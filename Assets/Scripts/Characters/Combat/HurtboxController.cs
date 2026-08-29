using UnityEngine;

public class HurtboxController : MonoBehaviour
{
    [SerializeField] private GameObject hurtbox;
    private AttackMove currentMove;

    public void ActivateHitBox(AttackMove newMove)
    {
        hurtbox.SetActive(false);

        currentMove = newMove;

        transform.localScale = currentMove.hurtboxSize;

        hurtbox.SetActive(true);
    }

    public void DeactivateHitBox()
    {
        hurtbox.SetActive(false);
        transform.localScale = Vector3.one;
        currentMove = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        HealthSystem health = other.GetComponentInParent<HealthSystem>();

        if (health != null)
        {
            health.TakeDamage(currentMove.attackPow);
        }
    }
}

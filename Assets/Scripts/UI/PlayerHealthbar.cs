using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthbar : MonoBehaviour
{
    public Slider healthbar;

    private void Start()
    {
        FighterController[] fighters = FindObjectsByType<FighterController>();

        for (var i = 0; i < fighters.Length; i++)
        {
            if (fighters[i].inputType == InputType.Player)
            {
                HealthSystem hp = fighters[i].GetComponent<HealthSystem>();

                hp.OnHealthChanged += UpdateHealthBar;
            }
        }
    }

    public void UpdateHealthBar(int newValue)
    {
        healthbar.value = newValue;
    }
}

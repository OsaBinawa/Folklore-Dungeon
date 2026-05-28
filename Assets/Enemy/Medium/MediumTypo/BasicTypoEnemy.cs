using System;
using UnityEngine;

public class BasicTypoEnemy : EnemyUnit
{
    public static event Action<string> OnUseShield;
    private int currentShield;
    [SerializeField] private GameObject Vfx_Shield;//VFX atau apapun lah
    [SerializeField] private float shieldChance = 0.5f;

    public override void Act(PlayerUnit player)
    {
        Debug.Log(name + " TypoEnemy Act");

        if (UnityEngine.Random.value <= shieldChance && currentShield <= 0)
        {
            Debug.Log("Using Shield");
            UseShield();
            OnActionFinished();
            return;
        }

        Debug.Log("Using normal attack");
        base.Act(player);
    }


    private void UseShield()
    {
        currentShield = Mathf.RoundToInt(data.MaxHP * 0.5f);

        Debug.Log($"{name} used Shield: {currentShield}");
        OnUseShield?.Invoke(data.name + " used shield");
        
        anim.SetTrigger("Shield");
        if (Vfx_Shield != null) {Vfx_Shield.SetActive(true);}
        
    }

    public override void TakeDamage(int damage, ElementType element)
    {
        if (currentShield > 0)
        {
            int absorbed = Mathf.Min(currentShield, damage);
            currentShield -= absorbed;
            damage -= absorbed;

            Debug.Log($"Shield absorbed {absorbed}, remaining shield: {currentShield}");
        }

        if (damage > 0)
            base.TakeDamage(damage, element);
    }
}

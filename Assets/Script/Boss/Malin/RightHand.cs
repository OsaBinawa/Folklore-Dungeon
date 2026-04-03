using UnityEngine;

public class RightHand : MainHand
{
    public override void TakeDamage(int damage, ElementType element)
    {
        base.TakeDamage(damage, element);

        PlayerUnit player = FindFirstObjectByType<PlayerUnit>();

        if (player != null)
        {
            int counterDamage = Data.BaseDamage / 2;
            player.TakeDamage(counterDamage, element);
        }
    }
}

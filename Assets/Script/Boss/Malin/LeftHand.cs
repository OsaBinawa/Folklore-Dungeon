using UnityEngine;

public class LeftHand : MainHand
{
    private int turnCounter;
    private int bonusDamage;

    protected override void OnTurnEnd()
    {
        turnCounter++;

        if (turnCounter % 3 == 0)
        {
            bonusDamage += 5;
        }
    }

    protected override void ModifyDamage(ref int damage)
    {
        damage += bonusDamage;
    }
}

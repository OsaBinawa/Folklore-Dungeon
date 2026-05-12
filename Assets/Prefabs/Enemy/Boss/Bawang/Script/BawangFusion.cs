using UnityEngine;

public class BawangFusion : EnemyUnit
{

    public override void Act(PlayerUnit player)
    {
        base.Act(player);
    }

    // Animation Event
    public void BuffATK()
    {
        Debug.Log(name + " buffs ATK");
    }

    // Animation Event
    public void BuffSPD()
    {
        Debug.Log(name + " buffs SPD");
    }
}
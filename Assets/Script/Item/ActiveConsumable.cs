using UnityEngine;

[System.Serializable]
public class ActiveConsumable
{
    public ItemSO source;
    public int remainingTurns;

    public int atkMod;
    public int spdMod;
    public int hpMod;

    public ActiveConsumable(ItemSO item)
    {
        source = item;
        remainingTurns = item.duration;

        atkMod = item.AtkMod;
        spdMod = item.SPDMod;
        hpMod = item.HPMod;
    }
}

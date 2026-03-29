using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffRandomise : MonoBehaviour
{
    [SerializeField] private List<BuffSO> allBuffs;
    [SerializeField] private Slots playerSlots;
    private void Awake()
    {
        playerSlots = FindFirstObjectByType<Slots>();
    }
    public List<BuffSO> GetChoices(int count, int minRarity, int maxRarity)
    {
        var owned = playerSlots.OwnedBuffs;

        List<BuffSO> valid = allBuffs
            .Where(buff =>
                buff.rarity >= minRarity &&
                buff.rarity <= maxRarity &&
                (buff.stackable || !owned.Contains(buff))
            )
            .ToList();

        List<BuffSO> result = new();

        for (int i = 0; i < count; i++)
        {
            if (valid.Count == 0) break;

            int index = Random.Range(0, valid.Count);
            result.Add(valid[index]);

            valid.RemoveAt(index); // prevent duplicate in same roll
        }

        return result;
    }
}

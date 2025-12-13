using UnityEngine;


[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int baseHP;
    [SerializeField] private int baseATK;
    [SerializeField] private int baseSpeed;

    private int finalHP;
    private int finalATK;
    private int finalSpeed;

    public int FinalHP => finalHP;
    public int FinalATK => finalATK;
    public int FinalSpeed => finalSpeed;

    public void Recalculate(Equipment weapon, Equipment armor)
    {
        finalHP = baseHP +
                  (weapon ? weapon.HPBonus : 0) +
                  (armor ? armor.HPBonus : 0);

        finalATK = baseATK +
                   (weapon ? weapon.ATKBonus : 0) +
                   (armor ? armor.ATKBonus : 0);

        finalSpeed = baseSpeed +
                     (weapon ? weapon.SpeedBonus : 0) +
                     (armor ? armor.SpeedBonus : 0);
    }
}

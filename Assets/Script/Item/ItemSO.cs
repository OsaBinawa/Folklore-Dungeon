using UnityEngine;

[CreateAssetMenu(menuName ="ItemSO")]
public class ItemSO : ScriptableObject
{
    public string Name;
    public int Heal;
    public int AtkMod;
    public int SPDMod;
    public int HPMod;
    public int duration = 3;
    public Sprite Sprite;
}

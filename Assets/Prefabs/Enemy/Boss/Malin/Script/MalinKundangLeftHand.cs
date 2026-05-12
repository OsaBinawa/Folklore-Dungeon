using UnityEngine;

public class MalinKundangLeftHand : MalinKundangHand
{
    [SerializeField] private float counterChance = 0.3f;

    public override void TakeDamage(int damage, ElementType element)
    {
        base.TakeDamage(damage, element);

        if (Random.value <= counterChance)
        {
            PlayerUnit player = FindFirstObjectByType<PlayerUnit>();

            if (player != null)
            {
                Debug.Log($"{name} counterattacks!");
                player.TakeDamage(data.BaseDamage, ElementType.Typo);
            }
        }
    }
}
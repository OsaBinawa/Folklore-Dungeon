using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DamageVFX : MonoBehaviour
{
    [SerializeField] private Image damageImage;
    private Tween damageTween;

    private void OnEnable()
    {
        PlayerUnit.OnPlayerDamaged += ShowDamage;
    }

    private void OnDisable()
    {
        PlayerUnit.OnPlayerDamaged -= ShowDamage;
    }

    public void ShowDamage()
    {
        damageTween?.Kill();

        Color color = damageImage.color;
        color.a = 0.5f;

        damageImage.color = color;

        damageTween = damageImage
            .DOFade(0f, 0.4f)
            .SetEase(Ease.OutQuad);
    }

    private void OnDestroy()
    {
        damageTween?.Kill();
    }
}
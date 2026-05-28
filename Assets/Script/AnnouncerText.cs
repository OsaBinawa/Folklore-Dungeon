using UnityEngine;
using DG.Tweening;
using TMPro;

public class AnnouncerText : MonoBehaviour
{
    [SerializeField] private AudioClip anouncementSFX;
    private SoundManager soundManager;
    private TMP_Text announcerText;
    private Tween textTween;

    private void Awake()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
        announcerText = GetComponent<TMP_Text>();
        announcerText.alpha = 0;
    }

    private void OnEnable()
    {
        PlayerUnit.OnInsufficientSkillPoint += ShowText;
        PlayerUnit.OnAttackNoTarget += ShowText;
        PlayerUnit.OnPlayerBasicAttack += ShowText;
        PlayerUnit.OnPlayerSkill += ShowText;
        PlayerUnit.OnUltimateEnergyInsufficient += ShowText;
        PlayerUnit.OnPlayerUltimate += ShowText;
        PlayerUnit.OnPlayerHeal += ShowText;
        PlayerUnit.OnPlayerUsedItem += ShowText;
        EnemyUnit.OnEnemyAct += ShowText;
        BasicBrokenEnemy.OnShredProgress += ShowText;
        EliteMissEnemy.OnInflictSlow += ShowText;
        EliteTypoEnemy.OnCounterAttack += ShowText;
    }

    private void OnDisable()
    {
        PlayerUnit.OnInsufficientSkillPoint -= ShowText;
        PlayerUnit.OnAttackNoTarget -= ShowText;
        PlayerUnit.OnPlayerBasicAttack -= ShowText;
        PlayerUnit.OnPlayerSkill -= ShowText;
        PlayerUnit.OnUltimateEnergyInsufficient -= ShowText;
        PlayerUnit.OnPlayerUltimate -= ShowText;
        PlayerUnit.OnPlayerHeal -= ShowText;
        PlayerUnit.OnPlayerUsedItem -= ShowText;
        EnemyUnit.OnEnemyAct -= ShowText;
        BasicBrokenEnemy.OnShredProgress -= ShowText;
        EliteMissEnemy.OnInflictSlow -= ShowText;
        EliteTypoEnemy.OnCounterAttack -= ShowText;
    }

    private void ShowText(string text)
    {
        textTween?.Kill();

        announcerText.text = text;

        soundManager.PlaySFX(anouncementSFX);

        Sequence seq = DOTween.Sequence();

        seq.Append(announcerText.DOFade(1f, 0.2f));
        seq.AppendInterval(1f);
        seq.Append(announcerText.DOFade(0f, 0.3f));

        textTween = seq;
    }

    private void OnDestroy()
    {
        textTween?.Kill();
    }
}
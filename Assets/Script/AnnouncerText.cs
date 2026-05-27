using UnityEngine;
using DG.Tweening;
using TMPro;

public class AnnouncerText : MonoBehaviour
{
    private TMP_Text announcerText;
    private Tween textTween;

    private void Awake()
    {
        announcerText = GetComponent<TMP_Text>();
        announcerText.alpha = 0;
    }

    private void OnEnable()
    {
        PlayerUnit.OnInsufficientSkillPoint += Showtext;
    }

    private void OnDisable()
    {
        PlayerUnit.OnInsufficientSkillPoint -= Showtext;
    }

    private void Showtext(string text)
    {
        textTween?.Kill();

        announcerText.text = text;

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
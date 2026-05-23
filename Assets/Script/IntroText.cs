using UnityEngine;
using DG.Tweening;
using TMPro;

public class IntroText : MonoBehaviour
{
    [SerializeField] private TMP_Text clickText;
    private Tween fadeTween;
    private Tween blinkTween;

    private void Awake()
    {
        BlinkText();
    }

    public void CloseIntro(CanvasGroup canvasGroup)
    {
        fadeTween?.Kill();
        fadeTween = canvasGroup.DOFade(0, 1).OnComplete(() => {Destroy(gameObject);});
    }

    private void BlinkText()
    {
        blinkTween?.Kill();
        clickText.DOFade(0.5f, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void OnDestroy()
    {
        fadeTween?.Kill();
        blinkTween?.Kill();
    }
}

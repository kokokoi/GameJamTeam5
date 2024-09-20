using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class BlinkCanvasGroup : MonoBehaviour
{
    public float DurationSeconds;
    public Ease EaseType;

    private CanvasGroup canvasGroup;

    private bool loop = false;

    void Start()
    {
        this.canvasGroup = this.GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if(loop)
        {
            this.canvasGroup.DOFade(0.0f, this.DurationSeconds).SetEase(this.EaseType).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
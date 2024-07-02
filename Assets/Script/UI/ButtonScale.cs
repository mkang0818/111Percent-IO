using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform myImageRect;

    private Vector3 originalScale;

    void Start()
    {
        // 현재 크기 저장
        originalScale = myImageRect.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 이미지 1.1배 확대
        myImageRect.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 이미지 원래 크기 복원
        myImageRect.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
    }
}
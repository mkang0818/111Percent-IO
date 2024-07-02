using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform myImageRect;

    private Vector3 originalScale;

    void Start()
    {
        // ���� ũ�� ����
        originalScale = myImageRect.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // �̹��� 1.1�� Ȯ��
        myImageRect.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // �̹��� ���� ũ�� ����
        myImageRect.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndText : MonoBehaviour
{
    Image imageToAnimate;
    public float minScale; // �ּ� ũ��
    public float maxScale; // �ִ� ũ��
    public float duration; // ���� �ð�

    void Start()
    {
        imageToAnimate = GetComponent<Image>();
        AnimateImage();
    }

    // �̹��� ũ�� �ִϸ��̼� ����
    void AnimateImage()
    {
        imageToAnimate.rectTransform.DOScale(minScale, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }
}

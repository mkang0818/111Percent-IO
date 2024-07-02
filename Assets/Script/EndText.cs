using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndText : MonoBehaviour
{
    Image imageToAnimate;
    public float minScale; // 최소 크기
    public float maxScale; // 최대 크기
    public float duration; // 지속 시간

    void Start()
    {
        imageToAnimate = GetComponent<Image>();
        AnimateImage();
    }

    // 이미지 크기 애니메이션 설정
    void AnimateImage()
    {
        imageToAnimate.rectTransform.DOScale(minScale, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }
}

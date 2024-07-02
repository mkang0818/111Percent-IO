using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CountDown : MonoBehaviour
{
    private Vector3 TextScale;

    private void Start()
    {
        TextScale = transform.localScale;
        CountDownOn();
    }

    // ī��Ʈ �ٿ� �ִϸ��̼�
    public void CountDownOn()
    {
        transform.DOScale(new Vector3(TextScale.x * 2, TextScale.y * 2, TextScale.z * 2), 0.5f);
        Invoke("BackUp", 0.5f);
    }

    void BackUp()
    {
        transform.DOScale(new Vector3(TextScale.x, TextScale.y, TextScale.z), 0.5f);
    }
}

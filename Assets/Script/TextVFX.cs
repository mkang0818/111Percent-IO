using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class TextVFX : MonoBehaviour
{
    private Vector3 TextScale;

    public TextMeshProUGUI ValueText;
    public string Value;
    private void Start()
    {
        ValueText.text = Value;
        TextScale = transform.localScale;
        TextAnim();
    }


    // 이펙트 텍스트 애니메이션
    public void TextAnim()
    {
        int size = GameManager.Instance.PlayerLv;
        transform.localScale = new Vector3(size, size, size);
        TextScale = transform.localScale;

        float RandScale = Random.Range(size, size + 2);
        transform.DOScale(new Vector3(TextScale.x + RandScale, TextScale.y + RandScale, TextScale.z + RandScale), 0.4f);
        Invoke("down", 0.5f);
        Invoke("ReturnText", 1f);
    }
    void down()
    {
        transform.DOScale(new Vector3(TextScale.x, TextScale.y, TextScale.z), 0.5f);
    }
    
    void ReturnText()
    {
        GetComponent<PoolObj>().ReleaseObject();
    }
}

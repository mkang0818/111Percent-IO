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

    // ����Ʈ �ؽ�Ʈ �ִϸ��̼�
    public void TextAnim()
    {
        int Textsize = GameManager.Instance.PlayerLv;
        transform.localScale = new Vector3(Textsize, Textsize, Textsize);
        TextScale = transform.localScale;

        float RandScale = Random.Range(Textsize, Textsize + 2);
        transform.DOScale(new Vector3(TextScale.x + RandScale, TextScale.y + RandScale, TextScale.z + RandScale), 0.4f);

        Invoke("BackUp", 0.5f);
        Invoke("ReleaseText", 1f);
    }
    void BackUp()
    {
        transform.DOScale(new Vector3(TextScale.x, TextScale.y, TextScale.z), 0.5f);
    }
    
    // Ǯ ������Ʈ ��ȯ
    void ReleaseText()
    {
        GetComponent<PoolObj>().ReleaseObject();
    }
}

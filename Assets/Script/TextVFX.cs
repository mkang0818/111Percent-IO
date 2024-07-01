using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TextVFX : MonoBehaviour
{
    private Vector3 TextScale;
    private void Start()
    {
        TextScale = transform.localScale;
        DamageTextOn();
    }
    void down()
    {
        transform.DOScale(new Vector3(TextScale.x, TextScale.y, TextScale.z), 0.5f);
    }
    public void DamageTextOn()
    {
        float RandScale = Random.Range(1.3f, 2);
        transform.DOScale(new Vector3(TextScale.x * RandScale, TextScale.y * RandScale, TextScale.z * RandScale), 0.4f);
        Invoke("down", 0.5f);
        Invoke("ReturnText", 1f);
    }
    void ReturnText()
    {
        Destroy(gameObject);
        //ReleaseObject();
    }
}

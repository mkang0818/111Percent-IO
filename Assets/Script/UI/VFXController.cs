using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    // ������Ʈ Ǯ ��ȯ
    public void ReleaseObj() => StartCoroutine(Release());
    IEnumerator Release()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<PoolObj>().ReleaseObject();
    }
}

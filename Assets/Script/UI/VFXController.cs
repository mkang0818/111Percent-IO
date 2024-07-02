using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    // 오브젝트 풀 반환
    public void ReleaseObj() => StartCoroutine(Release());
    IEnumerator Release()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<PoolObj>().ReleaseObject();
    }
}

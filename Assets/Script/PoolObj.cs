using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolObj : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }

    public void ReleaseObject()
    {
        if (!gameObject.activeSelf)
        {
            // 이미 반환된 경우, 추가 조치 없이 메서드 종료
            return;
        }
        else
        {
            Pool.Release(gameObject);
        }
    }
}

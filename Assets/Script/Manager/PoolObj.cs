using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolObj : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }


    // ������Ʈ Ǯ�� ��ȯ
    public void ReleaseObject()
    {
        if (!gameObject.activeSelf)
        {
            // �̹� ��ȯ�� ��� ����ó��
            return;
        }
        else
        {
            Pool.Release(gameObject);
        }
    }
}

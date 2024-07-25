using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;

    public Vector3[] TargetCamArr;

    void Update()
    {

        // ī�޶� Ÿ�� ����
        if (GameManager.Instance.isStart)
        {
            target = GameManager.Instance.Player.gameObject;
        }

        // ī�޶� Ÿ�� ������
        int CamIndex = GameManager.Instance.playerLv - 1;

        if (target != null && CamIndex >= 0)
        {
            transform.position = target.transform.position + TargetCamArr[CamIndex];
        }

    }
}
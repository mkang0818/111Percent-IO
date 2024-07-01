using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;

    public Vector3[] TargetCamArr;


    void Update()
    {
        // 카메라 타겟 설정
        if (GameManager.Instance.IsStart)
        {
            target = GameManager.Instance.player.gameObject;
        }

        // 카메라 타겟 움직임
        int CamIndex = GameManager.Instance.PlayerLv - 1;
        if (target != null && CamIndex >= 0)
        {
            transform.position = target.transform.position + TargetCamArr[CamIndex];
        }
    }
}
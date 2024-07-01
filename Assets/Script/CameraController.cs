using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;

    public Vector3[] TargetCamArr;


    void Update()
    {
        if (GameManager.Instance.IsStart)
        {
            target = GameManager.Instance.player.gameObject;
        }


        print(GameManager.Instance.PlayerLv);
        int CamIndex = GameManager.Instance.PlayerLv - 1;

        if (target != null && CamIndex >= 0)
        {
            print(CamIndex);
            transform.position = target.transform.position + TargetCamArr[CamIndex];
        }
    }
}
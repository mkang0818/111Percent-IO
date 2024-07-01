using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;

    public Vector3[] TargetCamArr;
    // Start is called before the first frame update
    void Start()
    {
        target = GameManager.Instance.player.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            print(GameManager.Instance.PlayerLv);
            int CamIndex = GameManager.Instance.PlayerLv - 1;
            print(CamIndex);
            transform.position = target.transform.position + TargetCamArr[CamIndex];
        }
    }


}